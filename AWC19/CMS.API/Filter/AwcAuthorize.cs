using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;


//namespace CMS.API.Filter
//{
//    public class CustomAuthenticationAttribute : ActionFilterAttribute, IAuthenticationFilter
//    {
//        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
//        {
//            //var user = context..HttpContext.User;

//            throw new NotImplementedException();
//        }
//    }
//using System.Web.Mvc;
//using System.Web.Http.Filters;
//using System.Web.Mvc.Filters;

namespace CMS.API.Filter
{

    //public class CustomAuthenticationAttribute : AuthorizeAttribute
    //{
    //public CustomAuthenticationAttribute()
    //{
    //}

    //public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
    //{
    //    //var user = context..HttpContext.User;

    //    throw new NotImplementedException();
    //}
    //}

    public class AllowedPath : IAuthorizationRequirement
    {

        public class AllowedPathHandler : AuthorizationHandler<AllowedPath>
        {
            private readonly IHttpContextAccessor contextAccessor;
            public AllowedPathHandler(IHttpContextAccessor contextAccessor)
            //public class RoleUrlValidationHandler(IHttpContextAccessor contextAccessor)
            {
                this.contextAccessor = contextAccessor;
            }



            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowedPath requirement)
            {
                if (!context.User.HasClaim(c => c.Type == ClaimTypes.UserData))
                    return Task.FromResult(0);

                // Contains Access String
                var userData = context.User.FindFirst(c => c.Type == ClaimTypes.UserData).Value;
                var accessPath = contextAccessor.HttpContext.Request.Path.Value.Substring(1);

                // Allowing ALL
                var accessDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(userData);
                if (!accessDict.ContainsKey("allowedpath"))
                    context.Fail();
                else if (!accessDict["allowedpath"].ToLower().Split('|').Contains(accessPath.ToLower()))
                    context.Fail();

                context.Succeed(requirement);
                return Task.FromResult(0);

                return Task.CompletedTask;
            }

            //protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowedPath requirement)
            //{
            //    throw new NotImplementedException();
            //}
        }
    }

    public class CustomAuthenticationAttribute : Attribute, IActionFilter
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public bool AllowMultiple { get; set; }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var x = context.Filters;


        }
    }


}
