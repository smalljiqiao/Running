using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 转成国际标准时间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ToUTC(this DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        /// <summary>
        /// 转成本地时间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ToLocal(this DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }



    }
}
