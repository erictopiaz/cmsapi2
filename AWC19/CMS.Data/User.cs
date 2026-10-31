using CMS.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Data
{
    #region 转到  ShareEntity
    ///// <summary>
    ///// 用户操作
    ///// </summary>
    //public class User
    //{
    //    /// <summary>
    //    /// 检测当前的账号是否有被风控每日下的活动
    //    /// 1：优惠红利， 2：积分  ， 3：反水， 4： 存款反点， 5：红包雨，6：周周挑战，7:每日摇奖
    //    /// 返回true,为禁止
    //    /// </summary>
    //    /// <param name="misc">用户的基本信息</param>
    //    /// <param name="controlIndex">1：优惠红利， 2：积分  ， 3：反水， 4： 存款反点， 5：红包雨，6：周周挑战, 7:每日摇奖</param>
    //    /// <returns></returns>
    //    public static bool ControllUser(string misc, int controlIndex = 0)
    //    {
    //        try
    //        {
    //            var miscModel = Newtonsoft.Json.JsonConvert.DeserializeObject<misc>(misc);
    //            if (string.IsNullOrEmpty(miscModel.promodisabled))
    //                return false;
    //            var controlIndexs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(miscModel.promodisabled);
    //            return controlIndexs.Contains(controlIndex);
    //        }
    //        catch (Exception)
    //        {
    //            return false;
    //        }
    //    }

    //}
    ///// <summary>
    ///// 系统维护信息提示
    ///// </summary>
    //public class SystemMaintance
    //{
    //    /// <summary>
    //    /// 检测当前的账号是否有被风控每日下的活动
    //    /// [{id:0,label:"注册维护",c:"系统维护",s:1}]
    //    /// 0：注册维护,1:支付宝定额提款维护中,2:注册短信验证关， 3：绑卡短信验证关
    //    /// 返回true,功能正在维护中
    //    /// </summary>
    //    /// <param name="misc">用户的基本信息</param>
    //    /// <param name="controlIndex">0：注册维护</param>
    //    /// <returns></returns>
    //    public static Tuple<bool, string> ConformMaintance(object body, int controlIndex = 0)
    //    {
    //        try
    //        {
    //            if (body == null)
    //                return new Tuple<bool, string>(false, "");

    //            var maintanceModel = JsonConvert.DeserializeObject<List<MaintanceStruct>>(body.ToString());

    //            foreach(var m in maintanceModel)
    //            {
    //                if (m.id == controlIndex && m.s == 1)
    //                    return new Tuple<bool, string>(true, m.c);
    //            }

    //            return new Tuple<bool, string>(false, "");
    //        }
    //        catch (Exception)
    //        {
    //            return new Tuple<bool, string>(false, "");
    //        }
    //    }

    //    public static Tuple<bool, string> ConformMaintanceDomain(object body, string host)
    //    {
    //        try
    //        {
    //            if (body == null)
    //                return new Tuple<bool, string>(false, "");

    //            if(body.ToString().IndexOf(host) >=0) //找到正在维护的域名了
    //                return new Tuple<bool, string>(true, "");

    //            return new Tuple<bool, string>(false, "");
    //        }
    //        catch (Exception)
    //        {
    //            return new Tuple<bool, string>(false, "");
    //        }
    //    }

    //}

    //public class MaintanceStruct
    //{
    //    public int id { get; set; }
    //    public string label { set; get; }
    //    public string c { set; get; }
    //    public int s { get; set; }
    //}

    //public class DomainBlackModel
    //{
    //    public string url { set; get; }
    //}

    #endregion
}
