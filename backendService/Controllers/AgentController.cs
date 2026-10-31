using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backendService.Controllers
{
    /// <summary>
    /// 代理管理
    /// </summary>
    [Produces("application/json")]
    [Route("api/Agent/[action]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
    }
}