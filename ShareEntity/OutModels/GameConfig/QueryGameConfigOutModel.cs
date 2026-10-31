using System;
using System.Collections.Generic;
using System.Text;

namespace ShareEntity.OutModels.GameConfig
{
    public class QueryGameConfigOutModel
    {
        //gameid,gamename,gamestatus,class as gameclass, params as param

        /// <summary>
        /// 游戏ID
        /// </summary>
        public int gameid { set; get; }
        /// <summary>
        /// 游戏名称
        /// </summary>
        public string gamename { set; get; }
        /// <summary>
        /// 游戏分类
        /// </summary>
        public int gameclass { set; get; }
        /// <summary>
        /// 当前启用状态
        /// </summary>
        public int gamestatus { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string createtime { set; get; }
        /// <summary>
        /// 创建人员
        /// </summary>
        public string who_create {set;get;}
    }

    public class QueryGameConfigParamsOutModel : QueryGameConfigOutModel
    {
        public string param { set; get; }
    }
}
