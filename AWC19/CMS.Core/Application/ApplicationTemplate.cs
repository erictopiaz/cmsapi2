







//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace AWC2019.Core.Application.Admin.Queries
//{
//    public class LoginRequest
//    {

//    }
//}



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

namespace AWC2019.Core.Application.Account.Queries
{
    /// <summary>
    /// MediatR Request that returns a List of LBModel
    /// </summary>
    public class SampleQuery : IRequest<SampleReturn>
    {
        public string Account { get; set; }
    }

    public class SampleReturn
    {
    }

    public class SampleValidator : AbstractValidator<SampleQuery>
    {
        public SampleValidator()
        {
            RuleFor(x => x.Account).NotNull();
            RuleFor(x => x.Account).MaximumLength(6);
        }
    }


    //public class SampleHandler : IRequestHandler<SampleQuery, SampleReturn>
    //{
    //    private readonly CMS.Data.BetDB _context;
    //    private readonly IMapper _mapper;
    //    public SampleHandler(CMS.Data.BetDB context, IMapper mapper)
    //    {
    //        _context = context;
    //        _mapper = mapper;
    //    }

    //    public Task<SampleReturn> Handle(SampleQuery request, CancellationToken cancellationToken)
    //    {

    //        using (SqlConnection connection = new SqlConnection(CMS.Data.BetDB.CONNECTIONSTRING))
    //        {
    //            var myEvent = connection.Query<LB>("SELECT * FROM LB WHERE account=@account",
    //                new { account = request.Account });
    //            var mappedResult1 = _mapper.Map<LB[], ViewModel.LBModel[]>(myEvent.ToArray());
    //            return Task.FromResult(new SampleReturn());
    //        }

    //        // DBCONTEXT METHOD
    //        var result = _context.LBs.Where(x => x.Account == request.Account);
    //        var mappedResult = _mapper.Map<LB[], ViewModel.LBModel[]>(result.ToArray());
    //        return Task.FromResult(new SampleReturn());
    //    }

    //}
}

