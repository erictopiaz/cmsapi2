using CMS.API.BLL.Promo;
using ShareEntity.InputModels.Promo;
using LogLib.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsLib;
using UtilsLib.JWT;
using UtilsLib.RequestHelper;

namespace CMS.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Spread/[action]")]
    [Authorize(Policy = "PathBasedPolicy")]
    public class SpreadController : ControllerBase
    {

        private readonly WriteToFile _writeToFile;
        private IHttpContextAccessor _accessor;
        private IConfiguration _config;


        #region Arvin 给重调整初始化
        public SpreadController(WriteToFile writeToFile, IHttpContextAccessor accessor, IConfiguration config)
        {
            _writeToFile = writeToFile;
            _accessor = accessor;
            _config = config;
        }
        #endregion



        
    }
}
