using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HB.Common.Core.Framework.Helpers
{
    /// <summary>
    /// Json序列化帮助类
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>json字符串</returns>
        public static String ToJson(object o)
        {
            return JsonHelper.ToJson(o, null);
        }

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="settings">序列化配置</param>
        /// <returns>json字符串</returns>
        public static String ToJson(object o, Newtonsoft.Json.JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                settings = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    DateFormatString = "yyyy/MM/dd HH:mm:ss.fff",
                    Formatting = Newtonsoft.Json.Formatting.None
                };
            }
            String jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(o, settings);
            return jsonString;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">反序列化类型</typeparam>
        /// <param name="jsonString">json字符串</param>
        /// <returns>序列化结果对象</returns>
        public static T ToObject<T>(String jsonString)
        {
            T result = default(T);
            if (jsonString != null)
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
            }
            return result;
        }

        /// <summary>
        /// 反序列化集合
        /// </summary>
        /// <typeparam name="T">反序列化类型</typeparam>
        /// <param name="jsonString">json字符串</param>
        /// <returns>反序列化集合</returns>
        public static List<T> ToObjectList<T>(String jsonString)
        {
            List<T> result = null;
            if (jsonString != null)
            {
                Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(jsonString);
            }
            return result;
        }

        /// <summary>
        /// 将Json字符串保存为文件
        /// </summary>
        /// <param name="jsonString">json字符串</param>
        /// <param name="fullName">文件全名（包括路径）</param>
        /// <returns></returns>
        public static bool ToJsonFile(string jsonString, string fullName)
        {
            return ToJsonFile(jsonString, fullName, new UTF8Encoding());
        }

        /// <summary>
        /// 将Json字符串保存为文件
        /// </summary>
        /// <param name="jsonString">json字符串</param>
        /// <param name="fullName">文件全名（包括路径）</param>
        /// <param name="encoding">编码类型:默认为UTF8</param>
        /// <returns></returns>
        public static bool ToJsonFile(string jsonString, string fullName, Encoding encoding)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return false;
            }
            if (encoding == null) { encoding = new UTF8Encoding(); }
            Byte[] jsonBuffer = encoding.GetBytes(jsonString);
            using (FileStream jsonFile = new FileStream(fullName, FileMode.OpenOrCreate))
            {
                jsonFile.Write(jsonBuffer, 0, jsonBuffer.Length);
            }
            return true;
        }


        /// <summary>
        /// 从json文件中读取类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static T ToObjectFromJsonFile<T>(string fullName)
        {
            var jsonString = ToStringFromJsonFile(fullName, new UTF8Encoding());
            return ToObject<T>(jsonString);
        }

        /// <summary>
        /// 从json文件中读取json字符串
        /// </summary>
        /// <param name="fullName">文件全名（包括路径）</param>
        /// <param name="encoding">编码类型:默认为UTF8</param>
        /// <returns></returns>
        public static string ToStringFromJsonFile(string fullName, Encoding encoding)
        {
            string result = string.Empty;
            if (File.Exists(fullName))
            {
                if (encoding == null)
                {
                    encoding = new UTF8Encoding();
                }
                using (FileStream fileStream = new FileStream(fullName, FileMode.Open))
                {
                    var buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, (int)fileStream.Length);
                    result = encoding.GetString(buffer);
                }
            }
            return result;
        }

    }

    /// <summary>
    /// JsonHelper扩展
    /// </summary>
    public static class JsonHelperExtends
    {
        /// <summary>
        /// 普通对象转json
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJson(this object data)
        {
            string result = string.Empty;
            if (data == null)
            {
                return result;
            }
            result = JsonHelper.ToJson(data); ;
            return result;
        }

        /// <summary>
        /// json字符串转实际对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string value)
        {
            var t = default(T);
            if (value.IsNullOrEmpty_S())
            {
                return t;
            }

            t = JsonHelper.ToObject<T>(value);
            return t;
        }
    }
}
