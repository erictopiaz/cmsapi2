using LogLib.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogLib.Helper
{
    /// <summary>
    /// 日志到服务器文件
    /// </summary>
    public class WriteToFile
    {
        private FileOption _filePath;
        public WriteToFile(IOptions<FileOption> options)
        {
            _filePath = options.Value;
        }

        public void Done(string logType, string title, string body, bool append, string serviceName = "api")
        {
            Task.Run(async () => await Task.WhenAll(WriteAllToFile(logType, title, body, append, _filePath.FilePath)).ConfigureAwait(false));

        }

        //public async Task Done_old(string logType, string title, string body, bool append)
        //{
        //    var json = JsonConvert.SerializeObject(new { logType = logType, title = title, body = body, append = append, serviceName = string.Format(@"{0}_async", _filePath.FilePath) });
        //    await LogConsumer.Pub(json);
        //}

        private IEnumerable<Task> WriteAllToFile(string logType, string title, string body, bool append, string serviceName="")
        {
            var taskList = new List<Task>();
            try
            {
                taskList.Add(TxtWrite(logType, title, body, append));
            }
            catch (Exception)
            {

            }
            return taskList;
        }
        /// <summary>
        /// 写日志文件
        /// </summary>
        /// <param name="logType">当前日志是哪个种类</param>
        /// <param name="title">日志标题</param>
        /// <param name="body">日志内容</param>
        /// <param name="append">是否追加文件</param>
        /// <returns></returns>
        private async Task TxtWrite(string logType, string title, string body, bool append)
        {
            var dateString = DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd");
            var path = string.Format(@"{0}{1}/{2}", _filePath.FilePath, dateString, logType);
            try
            {
                if (Directory.GetParent(path).Exists == false)
                {
                    Directory.CreateDirectory(Directory.GetParent(path).FullName);
                }
                if (File.Exists(path) == false)
                {
                    FileStream fs = File.Create(path);
                    fs.Close();
                }
                using (StreamWriter sw = new StreamWriter(path, append, System.Text.Encoding.GetEncoding("utf-8")))
                {
                    sw.WriteLine("***************************************************");
                    sw.WriteLine("{0} : {1}", DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss fffff"), title);
                    sw.WriteLine("{0} : {1}", DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss fffff"), body);
                    sw.WriteLine("***************************************************");
                    sw.WriteLine("");
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
                //return true;
            }
            catch
            {
                await Task.Delay(1);
                //return false;
            }
        }

        //private async Task TxtWrite_old(string logType, string title, string body, bool append, string serviceName = "")
        //{
        //    var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { logType = logType, title = title, body = body, append = append, serviceName = serviceName });

        //    Task.Run(() =>
        //    {
        //        //回调通知，成功收到款
        //        LogConsumer.Pub(json);
        //    });
        //}



        public static void TxtWriteTo(string logType, string title, string body, bool append, string filePath= "Logs/cLogs")
        {
            var dateString = DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd");
            var path = string.Format(@"{0}/{1}_{2}", filePath,dateString, logType);
            try
            {
                if (Directory.GetParent(path).Exists == false)
                {
                    Directory.CreateDirectory(Directory.GetParent(path).FullName);
                }
                if (File.Exists(path) == false)
                {
                    FileStream fs = File.Create(path);
                    fs.Close();
                }
                using (StreamWriter sw = new StreamWriter(path, append, System.Text.Encoding.GetEncoding("utf-8")))
                {
                    sw.WriteLine("***************************************************");
                    sw.WriteLine("{0} : {1}", DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss fffff"), title);
                    sw.WriteLine("{0} : {1}", DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss fffff"), body);
                    sw.WriteLine("***************************************************");
                    sw.WriteLine("");
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
                //return true;
            }
            catch
            {
                //return false;
            }
        }

    }

    public class LogDataModel
    {
        public string logType { set; get; }

        public string title { set; get; }
        public string body { set; get; }

        public bool append { set; get; }
        /// <summary>
        /// 用来区分不同日志的目录
        /// </summary>
        public string serviceName { set; get; }

    }
}
