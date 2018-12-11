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

        #region 集合同步操作方法

        /// <summary>
        /// 移除指定listId内部的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="vallue"></param>
        public void ListRemove<T>(string key, T value)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var jsonValue = this.ConvertJson(value);
            db.ListRemove(key, jsonValue);
        }

        /// <summary>
        /// 获取指定key的list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> ListRange<T>(string key)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var redisValues = db.ListRange(key);
            var result = this.ConvertList<T>(redisValues);
            return result;
        }

        /// <summary>
        /// 追加列表元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        public void ListRightPush<T>(string key, T value)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var jsonValue = this.ConvertJson(value);
            var result = db.ListRightPush(key, jsonValue);
        }

        /// <summary>
        /// 获取列表最后一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListRightPop<T>(string key)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var redisValue = db.ListRightPop(key);
            var result = this.ConvertObj<T>(redisValue);
            return result;
        }

        /// <summary>
        /// 插入列表元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        public void ListLeftPush<T>(string key, T value)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var jsonValue = this.ConvertJson(value);
            var result = db.ListLeftPush(key, jsonValue);
        }

        /// <summary>
        /// 获取列表第一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListLeftPop<T>(string key)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var redisValue = db.ListLeftPop(key);
            var result = this.ConvertObj<T>(redisValue);
            return result;
        }

        /// <summary>
        /// 获取列表的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListLength(string key)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = db.ListLength(key);
            return result;
        }


        #endregion

        #region 集合异步操作方法

        /// <summary>
        /// 异步移除list中的某个值
        /// </summary>
        /// <typeparam name="T">类型对象</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<long> ListRemoveAsync<T>(string key, T value)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var valueJson = this.ConvertJson(value);
            var result = await db.ListRemoveAsync(key, valueJson);
            return result;
        }

        /// <summary>
        /// 获取list数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> ListRangeAsync<T>(string key)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var redisValues = await db.ListRangeAsync(key);
            var result = this.ConvertList<T>(redisValues);
            return result;
        }

        /// <summary>
        /// 追加元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<long> ListRightPushAsync<T>(string key, T value)
        {
            key = this.WrapKey(key);
            var jsonValue = this.ConvertJson(value);
            var db = this.GetDb();
            var result = await db.ListRightPushAsync(key, jsonValue);
            return result;
        }

        /// <summary>
        /// 获取列表最后一个元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">值</param>
        /// <returns></returns>
        public async Task<T> ListRightPopAsync<T>(string key)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var redisValue = await db.ListRightPopAsync(key);
            var result = this.ConvertObj<T>(key);
            return result;
        }

        /// <summary>
        /// 追加元素到第一个位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<long> ListLeftPushAsync<T>(string key, T value)
        {
            key = this.WrapKey(key);
            var jsonValue = this.ConvertJson(value);
            var db = this.GetDb();
            var result = await db.ListLeftPushAsync(key, jsonValue);
            return result;
        }

        /// <summary>
        /// 获取列表第一个元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">值</param>
        /// <returns></returns>
        public async Task<T> ListLeftPopAsync<T>(string key)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var redisValue = await db.ListLeftPopAsync(key);
            var result = this.ConvertObj<T>(key);
            return result;
        }

        /// <summary>
        /// 获取位置元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> ListLengthAsync(string key)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = await db.ListLengthAsync(key);
            return result;
        }
        #endregion

        #region Hash操作同步方法

        /// <summary>
        /// 判断某个数据是否已缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashExists(string key, string dataKey)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = db.HashExists(key, dataKey);
            return result;
        }

        /// <summary>
        /// 设置hash数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string dataKey, T value)
        {
            key = this.WrapKey(key);
            var valueJson = this.ConvertJson(value);
            var db = this.GetDb();
            var result = db.HashSet(key, dataKey, valueJson);
            return result;
        }

        /// <summary>
        /// 删除hash
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string dataKey)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = db.HashDelete(key, dataKey);
            return result;
        }

        /// <summary>
        /// 获取Hash值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string dataKey)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var valueJson = db.HashGet(key, dataKey);
            var result = this.ConvertObj<T>(valueJson);
            return result;
        }

        /// <summary>
        /// Hash增量计数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey"></param>
        /// <param name="val">增量</param>
        /// <returns>增加之后的值</returns>
        public double HashIncrement(string key, string dataKey, double val = 1d)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = db.HashIncrement(key, dataKey, val);
            return result;
        }

        /// <summary>
        /// Hash减量计数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey"></param>
        /// <param name="val">减量</param>
        /// <returns>减完之后的值</returns>
        public double HashDecrement(string key, string dataKey, double val = 1d)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = db.HashDecrement(key, dataKey, val);
            return result;
        }

        /// <summary>
        /// 获取HashKey 所有rediskey
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashKeys<T>(string key)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var redisValues = db.HashKeys(key);
            var result = this.ConvertList<T>(redisValues);
            return result;
        }

        #endregion

        #region Hash操作异步方法

        /// <summary>
        /// 判断某个数据是否已缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashExistsAsync(string key, string dataKey)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = await db.HashExistsAsync(key, dataKey);
            return result;
        }

        /// <summary>
        /// 设置hash数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync<T>(string key, string dataKey, T value)
        {
            key = this.WrapKey(key);
            var valueJson = this.ConvertJson(value);
            var db = this.GetDb();
            var result = await db.HashSetAsync(key, dataKey, valueJson);
            return result;
        }

        /// <summary>
        /// 删除hash
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(string key, string dataKey)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = await db.HashDeleteAsync(key, dataKey);
            return result;
        }

        /// <summary>
        /// 获取Hash值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<T> HashGetAsync<T>(string key, string dataKey)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var valueJson = await db.HashGetAsync(key, dataKey);
            var result = this.ConvertObj<T>(valueJson);
            return result;
        }

        /// <summary>
        /// Hash增量计数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey"></param>
        /// <param name="val">增量</param>
        /// <returns>增加之后的值</returns>
        public async Task<double> HashIncrementAsync(string key, string dataKey, double val = 1d)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = await db.HashIncrementAsync(key, dataKey, val);
            return result;
        }

        /// <summary>
        /// Hash减量计数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="dataKey"></param>
        /// <param name="val">减量</param>
        /// <returns>减完之后的值</returns>
        public async Task<double> HashDecrementAsync(string key, string dataKey, double val = 1d)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = await db.HashDecrementAsync(key, dataKey, val);
            return result;
        }

        /// <summary>
        /// 获取HashKey 所有rediskey
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> HashKeysAsync<T>(string key)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var redisValues = await db.HashKeysAsync(key);
            var result = this.ConvertList<T>(redisValues);
            return result;
        }


        #endregion

        #region SortedSet 操作方法---同步

        /// <summary>
        /// 添加序列元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="score">排序号</param>
        /// <returns></returns>
        public bool SortedSetAdd<T>(string key, T value, double score)
        {
            key = this.WrapKey(key);
            var valueJson = this.ConvertJson(value);
            var db = this.GetDb();
            var result = db.SortedSetAdd(key, valueJson, score);
            return result;
        }

        /// <summary>
        /// 删除序列元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool SortedSetRemove<T>(string key, T value)
        {
            key = this.WrapKey(key);
            var valueJson = this.ConvertJson(value);
            var db = this.GetDb();
            var result = db.SortedSetRemove(key, valueJson);
            return result;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">开始位置,默认为0</param>
        /// <param name="stop">结束位置，默认为-1（全部）</param>
        /// <returns></returns>
        public List<T> SortedSetRangeByRank<T>(string key, int start = 0, int stop = -1)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var redisValues = db.SortedSetRangeByRank(key, start, stop);
            var result = this.ConvertList<T>(redisValues);
            return result;
        }

        /// <summary>
        /// 序列长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = db.SortedSetLength(key);
            return result;
        }

        #endregion

        #region SortedSet 操作方法---异步

        /// <summary>
        /// 添加序列元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="score">排序号</param>
        /// <returns></returns>
        public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score)
        {
            key = this.WrapKey(key);
            var valueJson = this.ConvertJson(value);
            var db = this.GetDb();
            var result = await db.SortedSetAddAsync(key, valueJson, score);
            return result;
        }

        /// <summary>
        /// 删除序列元素
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<bool> SortedSetRemoveAsync<T>(string key, T value)
        {
            key = this.WrapKey(key);
            var valueJson = this.ConvertJson(value);
            var db = this.GetDb();
            var result = await db.SortedSetRemoveAsync(key, valueJson);
            return result;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="start">开始位置,默认为0</param>
        /// <param name="stop">结束位置，默认为-1（全部）</param>
        /// <returns></returns>
        public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key, int start = 0, int stop = -1)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var redisValues = await db.SortedSetRangeByRankAsync(key, start, stop);
            var result = this.ConvertList<T>(redisValues);
            return result;
        }

        /// <summary>
        /// 序列长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> SortedSetLengthAsync(string key)
        {
            key = this.WrapKey(key);
            var db = this.GetDb();
            var result = await db.SortedSetLengthAsync(key);
            return result;
        }

        #endregion
    }
}
