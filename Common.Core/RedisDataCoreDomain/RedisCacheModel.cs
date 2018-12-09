using System;
using System.Collections.Generic;
using System.Text;

namespace RedisDataCoreDomain
{
 public  class RedisCacheModel<T>
    {
        /// <summary>
        /// 缓存的数据
        /// </summary>
        public T CacheData { get; set; }

        /// <summary>
        /// 有效时间
        /// </summary>
        public DateTime Effective { get; set; }

    }
}
