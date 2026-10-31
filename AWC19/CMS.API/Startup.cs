using AutoMapper;
using Ching.DistributedTransaction.EFModel;
using CMS.API.Filter;
using CMS.API.Middleware;
using Dapper.FluentMap;
using DBLibs.WXDapper;
using FluentValidation.AspNetCore;
using LogLib.Config;
using LogLib.Helper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Middlewares;
using Middlewares.logMiddleware;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using NSwag;
using NSwag.Generation.Processors.Security;
using ShareEntity.OutputModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UtilsLib;
using static Dapper.SqlMapper;

namespace CMS.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region 缓存服务开启
            services.AddMemoryCache();
            #endregion

            #region 这块要去掉.
            // MAINDB
            //services.AddDbContext<ShareEntity.MainDB>(options => options.UseSqlServer(Configuration.GetConnectionString("MainDBConnection")));
            Data.MainDB.CONNECTIONSTRING = Configuration.GetConnectionString("DapperConnectionReadOnly");
            Data.MainDB.WITHWRITE_CONNECTIONSTRING = Configuration.GetConnectionString("DapperConnection");
            Data.Misc.REDIS_CONNECTIONSTRING = Configuration.GetConnectionString("RedisCache") ?? "";
            Data.MainDB.DapperConnectionLog = Configuration.GetConnectionString("DapperConnectionLog");

            #endregion
            #region 模型检测
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    //获取验证失败的模型字段 
                    var errors = actionContext.ModelState.Keys
                    .SelectMany(a => actionContext.ModelState[a].Errors.ToDictionary(k => a, v => v.ErrorMessage));
                    //设置返回内容
                    return new OkObjectResult(new ResponseModel<object>
                    {
                        code = 0,
                        message = errors.First().Key + ":" + $"{errors.First().Value}" + " 验证失败"
                    });
                };
            });
            #endregion
            #region 读取配置文件  ching 这里配置DAPPER 初始化信息
            DapperStringBase.ConnectionString = Configuration.GetConnectionString("DapperConnection");
            DapperStringBase.ConnectionStringReadOnly = Configuration.GetConnectionString("DapperConnectionReadOnly");
            DapperStringBase.ConnectionLogs = Configuration.GetConnectionString("DapperConnectionLog");
            DapperStringBase.ConnectionGameRecordString = Configuration.GetConnectionString("ConnectionGameRecordString");
            DapperStringBase.ConnectionGamePostgreString = Configuration.GetConnectionString("ConnectionGamePostgreString");
            //string gameBetPostgreConn = this.Configuration.GetConnectionString("ConnectionGamePostgreString");
            services.Configure<FileOption>(Configuration.GetSection("FileOptions"));
            services.AddSingleton<DapperHelper>();
            services.AddSingleton<WriteToFile>();
            services.AddSingleton<WriteToDB>();
            #endregion


            #region EFCore 
            services.AddDbContext<AmountRecordDbContext>(options =>
            {
                options.UseNpgsql(DapperStringBase.ConnectionGamePostgreString);
            });

            #endregion
          

            #region Automapper / DapperMapper
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Core.MainMapperProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            FluentMapper.Initialize(config =>
            {
                config.AddMap(new Core.DapperMapper.ofThirdpayMap());
                config.AddMap(new Core.DapperMapper.ofNetBankMap());
            });

            #endregion

            #region MediaTR
            services.AddMediatR(
                typeof(Core.Application.账号管理.Queries.存款账号).Assembly,
                typeof(Core.Application.Admin.Queries.LoginRequestQuery).Assembly
            );
            #endregion

            #region JWT Authentication
            string authSecret = Configuration.GetSection("Authentication").GetValue<string>("Secret");

            var key = Encoding.ASCII.GetBytes(authSecret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(x =>
           {
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(key),
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   ClockSkew = TimeSpan.Zero
               };
           });

            #endregion

            //services.AddAuthorization(x => x.AddPolicy("Over21",
            //              policy => policy.Requirements.Add(new MinimumAgeRequirement(21))));
            //services.AddSingleton<IAuthorizationHandler, AllowedPathHandler>();

            services.AddAuthorization(x => x.AddPolicy("PathBasedPolicy",
              policy => policy.Requirements.Add(new AllowedPath())));
            services.AddSingleton<IAuthorizationHandler, AllowedPath.AllowedPathHandler>();

            #region FluentValidation
            services.AddMvc(
                option =>
                {
                    //var policy = new AuthorizationPolicy(Filter.AllowedPath);
                    //option.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));

                    option.EnableEndpointRouting = false;
                    option.Filters.Add(typeof(ValidateModelAttribute));  // Adding this Filter Attrubute to ALL API 
                    //option.Filters.Add(typeof(Filter.AllowedPath));
                })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AWC2019.Core.Application.Account.Queries.GetLBBetRecordQuery>());
            #endregion

            #region Swagger
            services.AddOpenApiDocument(document =>
            {
                document.Title = "管理API";
                document.Version = "1.1";
                document.Description = "项目10/01/2019~11/01/2019";
                document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });

                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                //      new OperationSecurityScopeProcessor("JWT"));

              

            });
            #endregion           
            //services.AddSwaggerDocument();

            //services.AddSwaggerGen(c => {
            //    c.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Version = "v1",
            //        Title = "awc客户端API",
            //        Description = "客户端基本API 操作",
            //        TermsOfService = new Uri("https://example.com/terms"),
            //        Contact = new OpenApiContact
            //        {
            //            Name = "wx",
            //            Email = "chinghuang112@gmail.com",
            //            Url = new Uri("https://xxoo.com/spboyer"),
            //        },
            //        License = new OpenApiLicense
            //        {
            //            Name = "wx",
            //            Url = new Uri("https://example.com/license"),
            //        }
            //    });

            //    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //    //c.IncludeXmlComments(xmlPath);
            //    foreach (var item in XmlCommentsFilePath)
            //    {
            //        c.IncludeXmlComments(item);
            //    }
            //});

        }
        static List<string> XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                DirectoryInfo d = new DirectoryInfo(basePath);
                FileInfo[] files = d.GetFiles("*.xml");
                var xmls = files.Select(a => Path.Combine(basePath, a.FullName)).ToList();
                return xmls;
            }
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            //loggerFactory. = NLog.LogManager.LoadConfiguration("nlog.config");
            //if (env.IsDevelopment()) {
            //    app.UseDeveloperExceptionPage();
            //}

            loggerFactory.ConfigureNLog("nlog.config");
            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = new
                 List<string> { "index.html" }
            });
            app.UseStaticFiles();
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });


            #region 异常统一处理 ，测试
            app.UseCustomException(code: 0);
            #endregion

            app.UseAuthentication();

            #region 统一回应处理 暂时只是添加请求header中当前管理账号
            app.UseResponse();
            #endregion

            #region 日志统一处理 ，测试
            app.UseRequestResponseLogging(needLogBody: false);
            #endregion

           
            //app.UseMiddleware<GlobalExceptionHandler>();

            app.UseResponseCompression();

            app.UseMvc(
            //routes =>
            //    {
            //        routes.MapRoute(
            //            name: "default",
            //            template: "{controller=Home}/{action=Index}");
            //    }
            );

            // ==== SWAGGER
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
