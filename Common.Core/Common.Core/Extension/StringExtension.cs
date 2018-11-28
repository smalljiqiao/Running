
using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// String类的扩展方法
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 判断字符串是否为空字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>字符串长度</returns>
        public static bool IsNullOrEmpty_S(this string str)
        {
            return String.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 判断字符串不为null、""、String.Empty
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>结果</returns>
        public static bool IsNotNullOrEmpty(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取字符串长度(无需判断字符串是否为null)
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>字符串长度</returns>
        public static int Lengs_S(this string str)
        {
            if (str == null) { return 0; }
            return str.Length;

        }

        /// <summary>
        /// 数组字符串链接
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Join(this ICollection<string> collection, string s)
        {
            return string.Join(s, collection);
        }

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetEnumDesc(this Enum t)
        {
            return HB.Common.Core.Framework.Helpers.EnumHelper.GetEnumDesc(t);
        }
    }


}
