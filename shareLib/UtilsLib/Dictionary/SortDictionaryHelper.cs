using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace UtilsLib.Dictionary
{
    public class SortDictionaryHelper
    {
        public SortDictionaryHelper()
        {
        }
        //采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();
        //微信密钥
        private string wxkey = string.Empty;
        /**
        * 设置某个字段的值
        * @param key 字段名
         * @param value 字段值
        */
        public void SetValue(string key, object value)
        {
            m_values[key] = value;
        }
        /// <summary>
        /// 设置微信密钥
        /// </summary>
        /// <param name="wxkey"></param>
        public void SetWxKey(string wxkey)
        {
            this.wxkey = wxkey;
        }
        /**
        * 根据字段名获取某个字段的值
        * @param key 字段名
         * @return key对应的字段值
        */
        public object GetValue(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return o;
        }

        /**
         * 判断某个字段是否已设置
         * @param key 字段名
         * @return 若字段key已被设置，则返回true，否则返回false
         */
        public bool IsSet(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            if (null != o)
                return true;
            else
                return false;
        }

        /**
        * @将Dictionary转成xml
        * @return 经转换得到的xml串
        * @throws WxPayException
        **/
        public string ToXml()
        {
            //数据为空时不能转化为xml格式
            if (0 == m_values.Count)
            {
                //Log.Error(this.GetType().ToString(), "WxPayData数据为空!");
                //throw new WxPayException("WxPayData数据为空!");
            }

            string xml = "<xml>";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                //字段值不能为null，会影响后续流程
                if (pair.Value == null)
                {
                    //Log.Error(this.GetType().ToString(), "WxPayData内部含有值为null的字段!");
                    //throw new WxPayException("WxPayData内部含有值为null的字段!");
                }

                if (pair.Value.GetType() == typeof(int))
                {
                    xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                }
                else if (pair.Value.GetType() == typeof(string))
                {
                    //xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                    xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                }
                else//除了string和int类型不能含有其他数据类型
                {
                    //Log.Error(this.GetType().ToString(), "WxPayData字段数据类型错误!");
                    //throw new WxPayException("WxPayData字段数据类型错误!");
                }
            }
            xml += "</xml>";
            return xml;
        }

        /**
        * @将xml转为WxPayData对象并返回对象内部的数据
        * @param string 待转换的xml串
        * @return 经转换得到的Dictionary
        * @throws WxPayException
        */
        public SortedDictionary<string, object> FromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                //Log.Error(this.GetType().ToString(), "将空的xml串转换为WxPayData不合法!");
                //throw new WxPayException("将空的xml串转换为WxPayData不合法!");
                return null;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;


            foreach (string key in new List<string>(m_values.Keys))
            {
                m_values.Remove(key);
            }
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                m_values[xe.Name] = xe.InnerText;//获取xml的键值对到m_values内部的数据中
            }

            try
            {
                //错误是没有签名
                string return_code = m_values["return_code"].ToString();
                if (string.Compare(return_code, "SUCCESS") != 0)
                {
                    return null;
                }
                if (CheckSign())//验证签名通过
                {
                    return m_values;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //throw new WxPayException(ex.Message);
                return null;
            }
        }

        /**
        * @Dictionary格式转化成url参数格式
        * @ return url格式串, 该串不包含sign字段值
        */
        public string ToUrl()
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    //Log.Error(this.GetType().ToString(), "WxPayData内部含有值为null的字段!");
                    //throw new WxPayException("WxPayData内部含有值为null的字段!");
                }

                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }


        /**
        * @Dictionary格式化成Json
         * @return json串数据
        */
        public string ToJson()
        {
            string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(m_values);// JsonMapper.ToJson(m_values);
            return jsonStr;
        }

        /**
        * @values格式化成能在Web页面上显示的结果（因为web页面上不能直接输出xml格式的字符串）
        */
        public string ToPrintStr()
        {
            string str = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value == null)
                {
                    //Log.Error(this.GetType().ToString(), "WxPayData内部含有值为null的字段!");
                    //throw new WxPayException("WxPayData内部含有值为null的字段!");
                }

                str += string.Format("{0}={1}<br>", pair.Key, pair.Value.ToString());
            }
            //Log.Debug(this.GetType().ToString(), "Print in Web Page : " + str);
            return str;
        }

        /**
        * @生成签名，详见签名生成算法
        * @return 签名, sign字段不参加签名
        */
        public string MakeSign()
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += "&key=" + wxkey;// WxPayConfig.KEY;
                                   //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }
        /**
        * @生成签名，详见签名生成算法
        * @return 签名, sign字段不参加签名
        */
        public string MakeSign(string keyString)
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += "&" + keyString + "=" + wxkey;// WxPayConfig.KEY;
                                                 //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }
        /// <summary>
        /// 获取输入串的MD5加密串串,大写
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public string GetMd5(string inputStr)
        {
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(inputStr));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }
        /// <summary>
        /// 获取输入字串的MD5加密后的大写方式
        /// </summary>
        /// <param name="inputStr">加密前串</param>
        /// <returns></returns>
        public string GetMD5(string inputStr)
        {
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(inputStr));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }
        /**
        * 
        * 检测签名是否正确
        * 正确返回true，错误抛异常
        */
        public bool CheckSign()
        {
            //如果没有设置签名，则跳过检测
            if (!IsSet("sign"))
            {
                //Log.Error(this.GetType().ToString(), "WxPayData签名存在但不合法!");
                //throw new WxPayException("WxPayData签名存在但不合法!");
                return false;
            }
            //如果设置了签名但是签名为空，则抛异常
            else if (GetValue("sign") == null || GetValue("sign").ToString() == "")
            {
                //Log.Error(this.GetType().ToString(), "WxPayData签名存在但不合法!");
                //throw new WxPayException("WxPayData签名存在但不合法!");
                return false;
            }

            //获取接收到的签名
            string return_sign = GetValue("sign").ToString();

            //在本地计算新的签名
            string cal_sign = MakeSign();

            if (cal_sign == return_sign)
            {
                return true;
            }
            else
            {
                return false;
            }

            //Log.Error(this.GetType().ToString(), "WxPayData签名验证错误!");
            //throw new WxPayException("WxPayData签名验证错误!");
        }

        /**
        * @获取Dictionary
        */
        public SortedDictionary<string, object> GetValues()
        {
            return m_values;
        }
        /// <summary>
        /// 设置当前字典数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public bool SetValues(string json)
        {
            try
            {
                if (m_values.Count > 0)
                {
                    m_values.Clear();
                }
                m_values = Newtonsoft.Json.JsonConvert.DeserializeObject<SortedDictionary<string, object>>(json);
                return m_values.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 清空当前缓存数据
        /// </summary>
        /// <returns></returns>
        public bool ClearValues()
        {
            try
            {
                if (m_values.Count <= 0)
                    return true;
                else
                {
                    m_values.Clear();
                    return true;
                }

            }
            catch (Exception)
            {

                return false;
            }
        }
        /**
            * 生成随机串，随机串包含字母或数字
            * @return 随机串
            */
        public string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        private int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        /**
            * 根据当前系统时间加随机序列来生成订单号
             * @return 订单号
            */
        //public string GenerateOutTradeNo(string mch_id)
        //{
        //    Random random = new Random(GetRandomSeed());
        //    string refno = "";
        //    for (int i = 0; i < 4; i++)
        //    {
        //        refno += random.Next(9).ToString();
        //    }
        //    refno = DateTime.UtcNow.AddHours(8).ToString("yyMMddHHmmssfff") + refno;
        //    return string.Format("{0}{1}", mch_id, refno);
        //}

        //public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        //{
        //    //直接确认，否则打不开    
        //    return true;
        //}

    }
}
