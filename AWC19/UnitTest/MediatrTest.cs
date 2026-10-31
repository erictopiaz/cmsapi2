//using AutoMapper;
//using AWC2019.Core.Application.Account.Queries;
//using AWC2019.Core.Entities;
//using AWC2019.Core.ViewModel;
//using MediatR;
//using Moq;
//using NUnit.Framework;
//using System.Threading.Tasks;

//namespace Tests
//{
//    public class MediatrTest
//    {
//        AutoMapper.MapperConfiguration mapper;

//        [SetUp]
//        public void Setup()
//        {
//            //MediatR.
//            //mapper = new AutoMapper.MapperConfiguration(cfg => 
//            //    cfg.AddProfile<AWC2019.Core.MainMapperProfile>());
//        }



//        [Test]
//        public async Task MediatRCallTest()
//        {
//            //Arange
//            var mediator = new Mock<IMediator>();

//            GetOutboundCallListQuery command = new GetOutboundCallListQuery();
//            command.Account = "a";

//            GetOutboundCallListHandler handler = new GetOutboundCallListHandler(null);

//            //Act
//            var x = await handler.Handle(command, new System.Threading.CancellationToken());

//            //Asert
//            //Do the assertion
//            //something like:
//            Assert.Greater(x.Count, 4);
//            //return null;
//        }
//    }
//}