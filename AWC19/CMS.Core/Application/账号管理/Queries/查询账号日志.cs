using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using CMS.Data.Models;
using AWC2019.Core.ViewModel;
using FluentValidation;
using System.Data.SqlClient;
using Dapper;
using AWC2019.Core.Validation;
using Newtonsoft.Json;
using CMS.Core.Application.账号管理;
using Microsoft.Extensions.Logging;
using Dapper.Contrib.Extensions;
using ShareEntity.OutputModels;
using DBLibs.WXDapper;
using System.Data;
using CMS.Core.ViewModel;

namespace CMS.Core.Application.账号管理.Queries
{
    public class 查询账号日志 
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 账号类型
        /// </summary>
        public int accountType { set; get; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { set; get; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { set; get; }

        [JsonIgnore]
        public string Admin { get; set; }
    }

   
   

}
