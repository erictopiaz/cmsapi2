using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Core.Application.账号管理
{
    public abstract class BaseAccount<TRequest, TResponse> : BaseRequestHandler<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
    {
        public BaseAccount(string dbCconnectionString, IMapper mapper, ILoggerFactory logger) : base(dbCconnectionString, mapper, logger)
        {
        }
    }
}
