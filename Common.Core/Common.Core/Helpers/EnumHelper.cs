using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace HB.Common.Core.Framework.Helpers
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">枚举值</param>
        /// <returns></returns>
        public static string GetEnumDesc<T>(int value)
        {
            Type enumType = typeof(T);
            DescriptionAttribute descAttribute = null;
            string name = Enum.GetName(enumType, value);//获取枚举名称,根据枚举值
            if (name != null)
            {
                FieldInfo fieldInfo = enumType.GetField(name);//获取枚举字段信息,根据字段名
                if (fieldInfo != null)
                {
                    //获取枚举描述特性
                    descAttribute = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) as DescriptionAttribute;
                }
            }

            if (descAttribute != null && descAttribute.Description.IsNotNullOrEmpty())
            {
                return descAttribute.Description;
            }
            return String.Empty;
        }

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetEnumDesc(Enum e)
        {
            if (e == null)
            {
                return string.Empty;
            }
            DescriptionAttribute descAttribute = null;
            Type enumType = e.GetType();
            FieldInfo fieldInfo = enumType.GetField(e.ToString());//获取枚举字段信息,根据字段名
            if (fieldInfo != null)
            {
                //获取枚举描述特性
                descAttribute = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) as DescriptionAttribute;
            }
            if (descAttribute != null && descAttribute.Description.IsNotNullOrEmpty())
            {
                return descAttribute.Description;
            }
            return String.Empty;

        }

        /// <summary>
        /// 获取枚举描述列表，转换位键值对
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="isHasAll">是否包含全部</param>
        /// <param name="filterItem">过滤[描述]</param>
        /// <returns>枚举字典列表</returns>
        public static List<EnumKeyValue> EnumDescToList<T>(bool isHasAll, params string[] filterItem)
        {
            List<EnumKeyValue> list = new List<EnumKeyValue>();
            if (isHasAll)
            {
                list.Add(new EnumKeyValue() { Key = 0, Name = "全部" });
            }
            var type = typeof(T);
            var fieldsInfo = type.GetFields();
            if (fieldsInfo.HasData())
            {
                foreach (var item in fieldsInfo)
                {
                    var curDescriptionAttr = Attribute.GetCustomAttribute(item, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (curDescriptionAttr != null && curDescriptionAttr.Description.IsNotNullOrEmpty())
                    {
                        if (Array.IndexOf<String>(filterItem, curDescriptionAttr.Description) != -1)
                        {
                            continue;
                        }
                        EnumKeyValue value = new EnumKeyValue();
                        value.Key = (int)Enum.Parse(typeof(T), item.Name);
                        value.Name = curDescriptionAttr.Description;
                        list.Add(value);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取枚举值列表，转换位键值对
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isHasAll"></param>
        /// <param name="filterItem"></param>
        /// <returns></returns>
        public static List<EnumKeyValue> EnumValueToList<T>(bool isHasAll, params string[] filterItem)
        {
            List<EnumKeyValue> list = new List<EnumKeyValue>();
            if (isHasAll) { list.Add(new EnumKeyValue() { Key = 0, Name = "全部" }); };

            var values = Enum.GetValues(typeof(T));
            if (values != null)
            {
                foreach (int item in values)
                {
                    string name = Enum.GetName(typeof(T), item);
                    if (Array.IndexOf<string>(filterItem, name) != -1)
                    { continue; };

                    EnumKeyValue value = new EnumKeyValue() { Key = item, Name = name };
                    list.Add(value);
                }
            }
            return list;
        }
    }

    /// <summary>
    /// 枚举键值类
    /// </summary>
    public class EnumKeyValue
    {
        public int Key { get; set; }

        public string Name { get; set; }
    }
}
