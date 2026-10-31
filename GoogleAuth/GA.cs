using GoogleAuth.Models;
using GoogleAuthenticatorService.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAuth {
    public class GA {
        /// <summary>
        /// 获取谷歌登录，QR和密钥
        /// </summary>
        /// <param name="cmsAccount">管理员账号</param>
        /// <param name="googleKey">生成谷歌验证码的加密key</param>
        /// <returns></returns>
        public static async Task<GoogleAuthOutModel> LoadGoogleAuthData(string cmsAccount,
            string googleKey) 
        {
            try {
                await Task.Delay(1);
                var Authenticator = new TwoFactorAuthenticator();
                var setupInfo = Authenticator.GenerateSetupCode("AWC CMS GA",
                                                                cmsAccount,
                                                                googleKey,
                                                                250,
                                                                250);
                var qrCodeUrl = setupInfo.QrCodeSetupImageUrl;
                var manualCode = setupInfo.ManualEntryKey;

                return new GoogleAuthOutModel() {
                    qrCodeUrl = qrCodeUrl,
                    manualCode = manualCode
                };
            }
            catch (Exception) {
                
                return new GoogleAuthOutModel();
            }
        }

        /// <summary>
        /// 获取谷歌验证数据
        /// </summary>
        /// <param name="code">输入的谷歌验证器 显示的代码</param>
        /// <param name="googleKey">生成谷歌验证码的加密key</param>
        /// <returns></returns>
        public static async Task<bool> VerifyGoogleAuthCode(int code, string googleKey) 
        {
            try {
                await Task.Delay(1);
                var Authenticator = new TwoFactorAuthenticator();
                return Authenticator.ValidateTwoFactorPIN(googleKey, code.ToString());
            }
            catch (Exception ) {
                return false;
            }
        }
    }
}
