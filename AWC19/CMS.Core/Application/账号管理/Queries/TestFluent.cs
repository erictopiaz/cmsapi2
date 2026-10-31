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
using Newtonsoft.Json.Linq;

namespace CMS.Core.Application.账号管理.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class TestFluent : IRequest<TestFluentReturn>
    {
        //public string id { get; set; }
        public string Account { get; set; }
        //[JsonIgnore]
        //public string Admin { get; set; }
        //public string ipaddress { get; set; }
        //public string datetime { get; set; }
        public string realname { get; set; }
        public DateTime date { get; set; }
        public DateTime? nulldate{ get; set; }

    }

    public class TestFluentReturn
    {
    }

    /// <summary>
    /// API Model Validation
    /// </summary>
    public class TestFluentValidator : AbstractValidator<TestFluent>
    {
        public TestFluentValidator()
        {
            RuleFor(x => x.Account).Account();
            //    //RuleFor(x => x.Account).Account();
            //    RuleFor(x => long.Parse(x.id)).Id();
            //When(x => x.ipaddress != null, () =>
            //{

            //});

            //RuleFor(x => x.realname).Realname();
            //RuleFor(x => x.Account).AccountRegex();
            RuleFor(x => x.nulldate).Datetime();
            RuleFor(x => x.date).Datetime();

            //RuleFor(x => x.ipaddress).IP();
            //RuleFor(x => x.datetime).DateTimeStr();
        }
    }

    /// <summary>
    /// Request Handler
    /// </summary>
    public class TestFluentHandler : BaseAccount<TestFluent, TestFluentReturn>
    {
        public TestFluentHandler(IMapper mapper, ILoggerFactory logger) : base(Data.MainDB.CONNECTIONSTRING, mapper, logger) { }
        


        protected override TestFluentReturn Handle(TestFluent request)
        {
            throw new NotImplementedException();
        }
    }
}

