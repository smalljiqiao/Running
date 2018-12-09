
using System.Collections.Generic;
namespace System
{
    /// <summary>
    /// 集合类扩展
    /// </summary>
    public static class ICollectionExtension
    {
        /// <summary>
        /// 集合数量大于0
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static bool HasData(this ICollection<Object> objs)
        {
            if (objs != null && objs.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 集合数量大于0
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static bool HasData(this IList<Object> objs)
        {
            if (objs != null && objs.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 集合数量大于0
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static bool HasData(this List<object> objs)
        {
            if (objs != null && objs.Count > 0)
            {
                return true;
            }
            return false;
        }




    }
}
