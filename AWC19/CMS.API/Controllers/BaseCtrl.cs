using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System.Data.SqlClient;
using Dapper;
using System.Linq;

namespace CMS.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class BaseCtrl : ControllerBase
    {
        protected readonly ILogger _logger;
        protected readonly IMapper _mapper;
        protected readonly IMediator _mediator;

        public BaseCtrl(IMapper mapper, IMediator mediator, ILogger logger)
        {
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }


    }
}
