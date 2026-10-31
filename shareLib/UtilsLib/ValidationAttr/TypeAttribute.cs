using System;
using System.Collections.Generic;
using System.Text;

namespace UtilsLib.ValidationAttr
{
    /// <summary>
    /// 反射时提取当前的数据类型
    /// int , string ...
    /// 1:汉字
    /// </summary>
    public class TypeAttribute : Attribute
    {
        public TypeAttribute(int alias)
        {
            Alias = alias;
        }
        public int Alias
        {
            get { return _Alias; }
            set { _Alias = value; }
        }
        private int _Alias;
    }

}
