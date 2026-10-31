using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace UtilsLib.FTP
{
    /// <summary>
    /// 文件ftp 操作
    /// </summary>
    public class FTPHelper
    {
        private static readonly string userName = string.Format(@"awcpt__storage\$awcpt__storage");
        private static readonly string password = "sc0zibFl9cbLHvHWgJ7ruJ8espksx7rff5uPcvl4LgjkygoeJS4AE84fPjgr";
        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="ftpUrl">FTP Server</param>
        /// <param name="fileName"></param>
        /// <param name="uploadData"></param>
        public static bool FTPUpload(string fileName, string uploadData, string ftpUrl= "ftp://waws-prod-sg1-015.ftp.azurewebsites.windows.net/")
        {

            //上传到文件目录.
            string ftpFolder = "site/wwwroot/lmessage/";
            //上传数据
            byte[] fileBytes = Encoding.UTF8.GetBytes(uploadData);

            try
            {
                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl + ftpFolder + fileName);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                //Enter FTP Server credentials.
                request.Credentials = new NetworkCredential(userName, password);
                request.ContentLength = fileBytes.Length;
                request.UsePassive = true;
                request.UseBinary = true;
                request.ServicePoint.ConnectionLimit = fileBytes.Length;
                request.EnableSsl = false;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    requestStream.Close();
                }

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        /// <summary>
        /// ftp 下载文件数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="uploadData"></param>
        /// <param name="ftpUrl"></param>
        /// <returns></returns>

        public static string FTPDownload(string fileName, string ftpUrl = "ftp://waws-prod-sg1-015.ftp.azurewebsites.windows.net/")
        {
            try
            {
                string ftpFolder = "site/wwwroot/lmessage/";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl + ftpFolder + fileName);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                request.Credentials = new NetworkCredential(userName, password);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                var responseData = string.Empty;
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        responseData = reader.ReadToEnd();
                    }
                }
                
                response.Close();
                return responseData;
            }
            catch (Exception)
            {
                return string.Empty ;
            }
            
        }
    }
}
