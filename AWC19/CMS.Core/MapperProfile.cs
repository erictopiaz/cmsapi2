using AutoMapper;
using CMS.Data.Models;
using Dapper;
using Dapper.FluentMap.Mapping;
using Newtonsoft.Json.Linq;
using ShareEntity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Core
{ 
    public class MainMapperProfile : Profile
    {
        public MainMapperProfile()
        {
            //    CreateMap<LB, CMS.Core.ViewModel.LBModel>()
            //        .ForMember(dest => dest.Bet, opt => opt.MapFrom(src => src.bet_money))
            //        .ForMember(dest => dest.BetID, opt => opt.MapFrom(src => src.bet_id))
            //        .ForMember(dest => dest.Winnings, opt => opt.MapFrom(src => src.bet_winning))
            //        .ForMember(dest => dest.isWin, opt => opt.MapFrom(src => src.bet_win));

            //CreateMap<ofUser, Application.Account.Queries.GetLBBetRecordQuery>();
            //CreateMap<ofWithdrawal, CMS.Core.Application.提款管理.BaseWithdrawQuery>();
            //CreateMap<ofWithdrawal, CMS.Core.Application.提款管理.Queries.VIP1Return>();
            //CreateMap<ofWithdrawal, CMS.Core.Application.提款管理.Queries.VIP2Return>();
            //CreateMap<ofWithdrawal, CMS.Core.Application.提款管理.Queries.VIP3Return>();
            //CreateMap<ofWithdrawal, CMS.Core.Application.提款管理.Queries.VIP4Return>();
            CreateMap<ofThirdpay, CMS.Core.Application.网站管理.Queries.ThirdpayDetailReturn>();
            CreateMap<ofUser, Application.账号管理.Queries.账号详细Return>();

        }
    }

}