using AutoMapper;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        AutoMapper.MapperConfiguration mapper;

        [SetUp]
        public void Setup()
        {
            //mapper = new AutoMapper.MapperConfiguration(cfg =>
            //cfg.CreateMap<Domain.Entities.LB, Domain.ViewModel.LBModel>()
            //    .ForMember(dest => dest.Bet, opt => opt.MapFrom(src => src.bet_money))
            //    .ForMember(dest => dest.BetID, opt => opt.MapFrom(src => src.bet_id))
            //    .ForMember(dest => dest.Winnings, opt => opt.MapFrom(src => src.bet_winning))
            //    .ForMember(dest => dest.isWin, opt => opt.MapFrom(src => src.bet_win))
            //);
            //mapper = new AutoMapper.MapperConfiguration(cfg =>
            //    cfg.AddProfile<AWC2019.Core.MainMapperProfile>());
        }

        [Test]
        public void MapperConfigurationVerify()
        {
            //mapper.AssertConfigurationIsValid();
        }


        [Test]
        public void Encode()
        {

        }

            [Test]
        public void MapperDTOVerify()
        {

            //var lb = new LB()
            //{
            //    Account = "a",
            //    bet_id = 12345,
            //    bet_money = 12345,
            //    bet_no = 123456,
            //    bet_win = 123,
            //    bet_winning = 888,
            //    bet_type = 1
            //};
            //IMapper iMapper = mapper.CreateMapper();
            //var lbViewModel = iMapper.Map<LB, LBModel>(lb);

            //Assert.AreEqual(lbViewModel.Account, lb.Account);
            //Assert.AreEqual(lbViewModel.Bet, lb.bet_money);
        }
    }
}