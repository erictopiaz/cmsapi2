using AutoMapper;
using CMS.Core.Application;
using CMS.Core.ViewModel;
using CMS.Data;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMS.Core
{ 
    public abstract class BaseRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {

        protected readonly string _dbCconnectionString;
        protected readonly IWebHostEnvironment _env;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;
        protected readonly string? _admin;

        public BaseRequestHandler(string dbCconnectionString, IMapper mapper, ILoggerFactory logger)
        {
            _dbCconnectionString = dbCconnectionString;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(TResponse));
        }

        public BaseRequestHandler(string dbCconnectionString, IMapper mapper, ILoggerFactory logger, string admin)
        {
            _dbCconnectionString = dbCconnectionString;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(TResponse));
            _admin = admin;
        }


        public BaseRequestHandler(string dbCconnectionString, IMapper mapper, ILoggerFactory logger, IWebHostEnvironment env)
        {
            _dbCconnectionString = dbCconnectionString;
            _mapper = mapper;
            _logger = logger.CreateLogger(typeof(TResponse));
            //_admin = admin;
            _env = env;
        }


        Task<TResponse> IRequestHandler<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken)
            => Task.FromResult(Handle(request));

        /// <summary>
        /// Override in a derived class for the handler logic
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response</returns>
        protected abstract TResponse Handle(TRequest request);





    }

}
