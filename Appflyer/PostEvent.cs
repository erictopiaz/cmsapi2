using DBLibs.WXDapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtilsLib.RequestHelper;

namespace Appflyer {
    public class PostEvent {

        /// <summary>
        /// appflyer 呼叫过来的数据
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="appsflyer_id">afid</param>
        /// <param name="afModel"></param>
        /// <returns></returns>
        public static async Task<AppflyerResponseModel> DoneINR(string eventName, string appsflyer_id, string cuid,
            AppflyerConfigModel afModel) {
            var responseData = string.Empty;
            try {
                var postDataObj = new {
                    appsflyer_id = appsflyer_id,
                    customer_user_id = cuid,
                    eventName = eventName,
                    eventValue = new {
                        af_currency = "INR"
                    }
                };

                if(string.IsNullOrEmpty(eventName) ) {
                    return new AppflyerResponseModel() { result = false, responseData = "appsflyer_id null" };
                }

                var postJson = JsonConvert.SerializeObject(postDataObj);
                var headers = new Dictionary<string, string>() {
                    { "authentication", afModel.authentication },
                };
                var postUrl = string.Format(@"{0}/{1}", afModel.proxyUrl, afModel.app_id);
                responseData = await MRequest.RelayFormWithoutWebException(postUrl, "post", headers, true, postJson,
                    "", "application/json", isAllowedProxy: false, proxyUrl: "");

                return new AppflyerResponseModel() { result = responseData.ToLower().Equals("ok"), responseData = responseData };
            }
            catch (Exception ex) {
                return new AppflyerResponseModel() { result = false, responseData = responseData + " ex.msg:" + ex.Message };
            }
        }
    }

    // 老的地送地址：https://api3.appsflyer.com/inappevent

    //    const response = await fetch(
    //                "https://awapis-appflyer.azurewebsites.net/api/flyer/UpdateCuidAndEventName",
    //                {
    //                  method: "POST",
    //                  headers: {
    //                    "Content-Type": "application/json",
    //                  },
    //                  body: JSON.stringify({
    //cuid: userId,
    //                    afid: appsFlyerUID,
    //                    event_name: "enter_game",
    //                  }),
    //                }
    //              );



    public class AppflyerResponseModel {
        public bool result { get; set; }
        public string responseData { get; set; }
    }

    public class AppflyerConfigModel {
        public string app_id {  get; set; }
        public string authentication { get; set; }
        public string appsflyer_id { get; set; }
        public string eventName { get; set; }
        public string domain { get; set; }
        public string proxyUrl { get; set; }
    }
}
