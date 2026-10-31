using CMS.Core.Application.网站管理.Queries;
using DBLibs.WXDapper;
using NJsonSchema;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using System.Threading.Tasks;
using UtilsLib.JWT;

namespace CMS.API.BLL.Currency {
    public class CurrencyBLL {
        /// <summary>
        /// 货币权限检测（管理员查询，可见会员）
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <param name="currencycode"></param>
        /// <returns></returns>
        public static WebNoticeAddReturn QueryAccess(string token, string secret, int currencycode) {
            var currencys = JwtHelper.DESDeCode_ClaimValueByClaimKey(token: token, secret: secret);
            if (!currencys.Contains(currencycode)) {

                return new WebNoticeAddReturn() {
                    code = ShareEntity.Global.ResponseCode.code.FAIL,
                    message = "请选择货币查询"
                };
            }
            else {
                return new WebNoticeAddReturn() {
                    code = ShareEntity.Global.ResponseCode.code.SUCCESS,
                    message = ""
                };
            }

        }
    }
}
