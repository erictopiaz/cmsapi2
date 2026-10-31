using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot {
    public class WriteToTelegramBot {
        /// <summary>
        /// 去通知telegram bot
        /// </summary>
        /// <param name="txtMessage"></param>
        /// <param name="apiToken"></param>
        /// <param name="chat_Id"></param>
        /// <returns></returns>
        public static async Task NoticeBot(string txtMessage, string apiToken, string chat_Id) {
            try {
                //var txtMessage = "" +
                //       @"新注册" + "\n" +
                //       "账号：" + "ching0088" + "\n" +
                //       "注册时间：" + DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss");

                //txtMessage = string.Format(@"新注册 '\n' 账号：ching0088 '\n' 注册时间：{0}", DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"));

                var chatId = new ChatId(Convert.ToInt64(chat_Id));
                var tg = new Telegram.Bot.TelegramBotClient(apiToken);
                await tg.SendTextMessageAsync(chatId, txtMessage);
                //return true;
            }
            catch (Exception ex) {

            }
            
        }
    }
}
