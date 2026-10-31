using LogLib.Config;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;

namespace TestUnit
{
    public class JDB_TEST
    {

        string ACCOUNT = "john2602";
        int GAME_ID = 164;
        string DB_CONNECTION = "Server=tcp:h9hr3us8kngz.database.windows.net,1433;Initial Catalog=awcdb29atib1s;Persist Security Info=False;User ID=phHZ6gWlX7LY;Password=TFGqTOYIzDkC8AJB3myX9hePCynDg9mR;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;";
        //string DB_CONNECTION = "Server=tcp:VbKaiS5c51.database.windows.net,1433;Initial Catalog=awcdb1;Persist Security Info=False;User ID=YyKptp1p4k;Password=D3f1CzBsk6nveenS3seK;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;";
        LogLib.Helper.WriteToFile logFilePath;
        GameLibs.JDB.JDB game;
        [SetUp]
        public void Setup()
        {
            DBLibs.WXDapper.DapperStringBase.ConnectionString = DB_CONNECTION;
            DBLibs.WXDapper.DapperStringBase.ConnectionStringReadOnly = DB_CONNECTION;
            DBLibs.WXDapper.DapperStringBase.ConnectionGameRecordString = DB_CONNECTION;
            DBLibs.WXDapper.DapperStringBase.ConnectionLogs = DB_CONNECTION;

            logFilePath = new LogLib.Helper.WriteToFile(Options.Create(new FileOption()
                    {
                        FilePath = "test.log"
                    })
                );

            game = new GameLibs.JDB.JDB(logFilePath);
            game.account = ACCOUNT;
            game.gameId = GAME_ID;

        }

        [Test]
        public void JDB_GetBalance()
        {
            try
            {

                var temp = game.BalanceQueryAsync();
                Guid guid = new Guid();
                string refno = Guid.NewGuid().ToString().Replace("-", "");
                var result = temp.Result;
                var returnValue = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                Assert.IsEmpty(returnValue);
            }
            catch (Exception ex)
            {
                string test = ex.Message;
                Assert.Fail();
            }
            Assert.Pass();
        }


        [Test]
        public void JDB_TransferIN()
        {
            try
            {

                Guid guid = new Guid();
                string refno = Guid.NewGuid().ToString().Replace("-", "");

                var result = game.TransferDepositAsync("2",refno).Result;
                var returnValue = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                Assert.IsEmpty(returnValue);
            }
            catch (Exception ex)
            {
                string test = ex.Message;
                Assert.Fail();
            }
            Assert.Pass();
        }

        [Test]
        public void JDB_GameLogin()
        {
            try
            {

                Guid guid = new Guid();
                string refno = Guid.NewGuid().ToString().Replace("-", "");

                var result = game.LoginAsync(new CMS.Data.InputModels.Game.GameLoginBaseInputModel() { 
                    gameType = 7,
                    gamecode="7006"
                }).Result;
                var returnValue = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                Assert.IsEmpty(returnValue);
            }
            catch (Exception ex)
            {
                string test = ex.Message;
                Assert.Fail();
            }
            Assert.Pass();
        }
    }
}