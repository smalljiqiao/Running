using HB.Common.Core.Framework.Helpers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisDataCoreDomain
{
    public class RedisHelper
    {
        private readonly ConnectionMultiplexer _connection;

        private int DbNum { get; }
        /// <summary>
        /// key前缀
        /// </summary>
        public string PrefixKey;

        public RedisHelper(int dbNum=0):this(dbNum,null)
        {
        }
        /// <summary>
        /// 连接地址有默认地址，也有动态传入地址
        /// </summary>
        /// <param name="dbNum"></param>
        /// <param name="redisConnectionString"></param>
        public RedisHelper(int dbNum,string redisConnectionString)
        {
            this.DbNum = dbNum;
            _connection = string.IsNullOrWhiteSpace(redisConnectionString) ?
                RedisConnectionManager.Instance : RedisConnectionManager.GetManager(redisConnectionString);
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="func">委托</param>
        /// <returns>redis数据库</returns>
        private T Do<T>(Func<IDatabase, T> func)
        {
            var database = _connection.GetDatabase(DbNum);
            return func(database);
        }

        /// <summary>
        /// 获取数据库操作对象
        /// </summary>
        /// <returns></returns>
        public IDatabase GetDb()
        {
            var database = _connection.GetDatabase(DbNum);
            return database;
        }

        #region 数据格式类型转换
        /// <summary>
        /// 转换Json
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值(如为string类型,则直接返回)</param>
        /// <returns>json字符串</returns>
        private string ConvertJson<T>(T value)
        {
            if (value == null) { return string.Empty; }
            if (value is string) { return value.ToString(); }
            var result = JsonHelper.ToJson(value);
            return result;
        }
        /// <summary>
        /// 转为类型对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        private T ConvertObj<T>(RedisValue value)
        {
            return JsonHelper.ToObject<T>(value);
        }
        /// <summary>
        /// 转换为类型集合
        /// </summary>
        /// <typeparam name="T">类型对象</typeparam>
        /// <param name="values">值</param>
        /// <returns></returns>
        private List<T> ConvertList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            if (values != null && values.Length > 0)
            {
                foreach (var item in values)
                {
                    var model = this.ConvertObj<T>(item);
                    result.Add(model);
                }
            }
            return result;
        }
        /// <summary>
        /// keys转换
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        private RedisKey[] ConvertRedisKeys(List<string> redisKeys)
        {
            if (redisKeys != null && redisKeys.Count > 0)
                return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
            return new RedisKey[0];
        }
        #endregion

        #region key的功能
        /// <summary>
        /// 生成实际存储key
        /// </summary>
        /// <param name="oldKey">传入key</param>
        /// <returns>返回prefixKey+oldKey</returns>
        public string WrapKey(string oldKey)
        {
            var prefixKey = this.PrefixKey ?? RedisConnectionManager.SysPrefix;
            return new StringBuilder(prefixKey).Append(oldKey).ToString();
        }

        #endregion

        #region redis同步操作方法--------查询与添加


        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool Add(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            key = this.WrapKey(key);
            var result = this.Do(db => db.StringSet(key, value, expiry));
            return result;
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="keyValues">缓存</param>
        /// <returns>是否添加成功</returns>
        public bool Add(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            var newKeyValues = keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(this.WrapKey(p.Key), p.Value)).ToArray();
            var result = this.Do(db => db.StringSet(newKeyValues));
            return result;
        }


        /// <summary>
        /// 添加缓存对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="obj">对象</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool Add<T>(string key, T obj, TimeSpan? expiry = null)
        {
            var value = this.ConvertJson(obj);
            var result = this.Add(key, value, expiry);
            return result;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string Get(string key)
        {
            key = this.WrapKey(key);
            var result = this.Do(db => db.StringGet(key));
            return result;
        }
        #endregion

        #region redis异步操作方法----------查询与添加异步

        /// <summary>
        /// 异步添加缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        public async Task<bool> AddAsync(string key, string value, TimeSpan? expire = default(TimeSpan?))
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = await db.StringSetAsync(key, value, expire);
            return result;
        }

        /// <summary>
        /// 异步批量添加缓存
        /// </summary>
        /// <param name="keyValues">缓存键值对</param>
        /// <returns>是否添加成功</returns>
        public async Task<bool> AddAsync(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            var newKeyValues = keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(this.WrapKey(p.Key), p.Value)).ToArray();
            var db = this.GetDb();
            var result = await db.StringSetAsync(newKeyValues);
            return result;
        }

        /// <summary>
        /// 异步添加缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        public async Task<bool> AddAsync<T>(string key, T value, TimeSpan? expire = default(TimeSpan?))
        {
            string jsonString = this.ConvertJson(value);
            var result = await this.AddAsync(key, jsonString, expire);
            return result;
        }

        /// <summary>
        /// 异步获取缓存
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>结果</returns>
        public async Task<string> GetAsync(string key)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = await db.StringGetAsync(key);
            return result;
        }

        /// <summary>
        /// 批量获取缓存
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<RedisValue[]> GetAsync(ICollection<string> keys)
        {
            var newKeys = keys.Select(this.WrapKey).ToList();
            var db = this.GetDb();
            var redisKeys = this.ConvertRedisKeys(newKeys);
            var result = await db.StringGetAsync(redisKeys);
            return result;
        }

        /// <summary>
        /// 异步获取缓存
        /// </summary>
        /// <param name="key">类型</param>
        /// <returns>缓存对象</returns>
        public async Task<T> GetAsync<T>(string key)
        {
            var stringJson = await this.GetAsync(key);
            var result = this.ConvertObj<T>(stringJson);
            return result;
        }

        /// <summary>
        /// 异步增量计数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">增量</param>
        /// <returns>增加之后的结果</returns>
        public async Task<double> IncrementAsync(string key, double val = 1d)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = await db.StringIncrementAsync(key, val);
            return result;
        }

        /// <summary>
        /// 异步减量计数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">减值</param>
        /// <returns>减完之后的结果</returns>
        public async Task<double> DecrementAsync(string key, double val = 1d)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = await db.StringDecrementAsync(key, val);
            return result;
        }

        #endregion
       
        
     
    }
}
