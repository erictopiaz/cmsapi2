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
    public class GetLBBetRecordQuery : IRequest<List<LBModel>>
    {
        public string Account { get; set; }
        //public DateTime? StartTime { get; set; }
        //public DateTime? EndTime { get; set; }
    }


    public class GetLBBetRecordQueryValidator : AbstractValidator<GetLBBetRecordQuery>
    {
        public GetLBBetRecordQueryValidator()
        {
            RuleFor(x => x.Account).NotNull();
            RuleFor(x => x.Account).MaximumLength(6);
        }
    }


    //public class GetLBBetRecordtHandler : IRequestHandler<GetLBBetRecordQuery, List<LBModel>>
    //{
    //    private readonly CMS.Data.BetDB _context;
    //    private readonly IMapper _mapper;
    //    public GetLBBetRecordtHandler(CMS.Data.BetDB context, IMapper mapper)
    //    {
    //        _context = context;
    //        _mapper = mapper;
    //    }

    //    public Task<List<ViewModel.LBModel>> Handle(GetLBBetRecordQuery request, CancellationToken cancellationToken)
    //    {

    //        using (SqlConnection connection = new SqlConnection(CMS.Data.BetDB.CONNECTIONSTRING))
    //        {
    //            var myEvent = connection.Query<LB>("SELECT * FROM LB WHERE account=@account", 
    //                new { account=request.Account });
    //            var mappedResult1 = _mapper.Map<LB[], ViewModel.LBModel[]>(myEvent.ToArray());
    //            return Task.FromResult(new List<ViewModel.LBModel>(mappedResult1));
    //        }

    //        // DBCONTEXT METHOD
    //        var result =  _context.LBs.Where(x => x.Account == request.Account);
    //        var mappedResult = _mapper.Map<LB[], ViewModel.LBModel[]>(result.ToArray());
    //        return Task.FromResult(new List<ViewModel.LBModel>(mappedResult));
    //    }

    //}
}
