using AutoMapper;
using ShareEntity.OutputModels;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CMS.Core.ViewModel
{
    public class BaseActions
    {
        protected readonly CMS.Data.MainDB _context;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        public BaseActions(CMS.Data.MainDB context, IMapper mapper, ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        //public ResponseModel<T> ResponseFail(string message, T data)
        //{
        //    var result = new ResponseModel<T>()
        //    {
        //        code = ShareEntity.Global.ResponseCode.code.FAIL,
        //        message = message,
        //        Data = data
        //    };
        //    return result;
        //}

        //public ResponseModel<T> ResponseSuccess(string message, T data)
        //{
        //    var result = new ResponseModel<T>()
        //    {
        //        code = ShareEntity.Global.ResponseCode.code.SUCCESS,
        //        message = message,
        //        Data = data
        //    };
        //    return result;
        //}

    }
}



