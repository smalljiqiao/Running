using System;
using System.Collections.Generic;
using System.Text;

namespace HB.Common.Core.Framework.Helpers
{
    /// <summary>
    /// 随机码帮助类
    /// </summary>
    public class RandomHelper
    {
        private static char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private static char[] constantForValue = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        /// 获取数值验证码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetNumCode(int length = 4)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                newRandom.Append(constantForValue[rd.Next(10)]);
            }
            return newRandom.ToString();
        }
    }
}
