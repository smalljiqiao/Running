using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDBDataCore.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBDataCore
{
    public class MongodbClientService:IDisposable
    {

        private IConfiguration configuration;

        private MongodbHost mongodb;

        private MongoClient mongoClient;

        public MongodbClientService(IConfigurationRoot configurationRoot)
        {
            this.configuration = configurationRoot;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// new() 无参的构造函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMongoCollection<T> GetTable<T>() where T :class,new() {
            if (mongoClient == null)
                mongoClient = new MongoClient(configuration.GetSection("").Value);
            var tableinfo = GetDataTableInfo<T>();
            if (tableinfo == null)
                throw new Exception($"类型{typeof(T).ToString()}没有赋值数据库名和表名，请加特性");
            var database = this.mongoClient.GetDatabase(tableinfo.Database);
            if (database == null)
                throw new Exception($"数据库{tableinfo.Database}不存在");
            var table = database.GetCollection<T>(tableinfo.TableName);
            if (table == null)
                throw new Exception($"表{tableinfo.TableName}不存在");

            return table;
        }
        /// <summary>
        /// 通过特性获取数据库表信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public MongoTableAttribute GetDataTableInfo<T>() {
            var type = typeof(T);
            var attrs = type.GetCustomAttributes(typeof(T),true);
            if (attrs != null && attrs.Length > 0) {
                return attrs[0] as MongoTableAttribute;
            }
            return null;
        }


        private IMongoCollection<T> GetCollection<T>() where T : class, new()
        {
            return this.GetTable<T>();
        }

        #region 增删查改
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddAsync<T>(T data) where T : class, new()
        {
            try
            {
                await this.GetTable<T>().InsertOneAsync(data);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        /// <summary>
        /// 批量写入
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync<T>(List<T> datas) where T : class, new()
        {
            try
            {
                var client = this.GetTable<T>();
                await client.InsertManyAsync(datas);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        public async Task<UpdateResult> Update<T>(T data, string id) where T : class, new()
        {
            try
            {
                var client = this.GetTable<T>();
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                var updateList = new List<UpdateDefinition<T>>();

                data.GetType().GetProperties().ToList().ForEach(item =>
                {
                    var attrs = item.GetCustomAttributes(typeof(BsonIgnoreAttribute), false);
                    if (item.Name.ToLower() != "id" && attrs.Length == 0)
                    {
                        updateList.Add(Builders<T>.Update.Set(item.Name, item.GetValue(data)));
                    }
                });

                var updateFilter = Builders<T>.Update.Combine(updateList);
                return await client.UpdateOneAsync(filter, updateFilter);
            }
            catch (Exception ex)
            {
                throw new Exception("mongodb修改数据错误", ex);
            }
        }

        /// <summary>
        /// 批量修改数据
        /// </summary>
        /// <param name="dic">要修改的字段</param>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">修改条件</param>
        /// <returns></returns>
        public UpdateResult UpdateManay<T>(Dictionary<string, string> dic, FilterDefinition<T> filter) where T : class, new()
        {
            try
            {
                var client = this.GetCollection<T>();
                T t = new T();
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (!dic.ContainsKey(item.Name)) continue;
                    var value = dic[item.Name];
                    list.Add(Builders<T>.Update.Set(item.Name, value));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return client.UpdateMany(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 异步批量修改数据
        /// </summary>
        /// <param name="dic">要修改的字段</param>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">修改条件</param>
        /// <returns></returns>
        public async Task<UpdateResult> UpdateManayAsync<T>(Dictionary<string, string> dic, FilterDefinition<T> filter) where T : class, new()
        {
            try
            {
                var client = this.GetCollection<T>();
                T t = new T();
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (!dic.ContainsKey(item.Name)) continue;
                    var value = dic[item.Name];
                    list.Add(Builders<T>.Update.Set(item.Name, value));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return await client.UpdateManyAsync(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectId</param>
        /// <returns></returns>
        public async Task<DeleteResult> DeleteAsync<T>(string id) where T : class, new()
        {
            try
            {
                var client = this.GetCollection<T>();
                //修改条件
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                return await client.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw new Exception("Mongodb删除数据失败", ex);
            }

        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">删除的条件</param>
        /// <returns></returns>
        public async Task<DeleteResult> DeleteManyAsync<T>(FilterDefinition<T> filter) where T : class, new()
        {
            try
            {
                var client = this.GetCollection<T>();
                return await client.DeleteManyAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据条件获取总数
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public async Task<long> CountAsync<T>(FilterDefinition<T> filter) where T : class, new()
        {
            try
            {
                var client = this.GetCollection<T>();
                return await client.CountAsync(filter);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// 根据id查询一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectid</param>
        /// <returns></returns>
        public async Task<T> FindOneAsync<T>(string id, string[] field = null) where T : class, new()
        {
            try
            {
                var client = this.GetCollection<T>();
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    return await client.Find(filter).FirstOrDefaultAsync();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                return await client.Find(filter).Project<T>(projection).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public async Task<List<T>> FindListAsync<T>(FilterDefinition<T> filter, string[] field = null, SortDefinition<T> sort = null) where T : class, new()
        {
            try
            {
                var client = this.GetCollection<T>();
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return await client.Find(filter).ToListAsync();
                    return await client.Find(filter).Sort(sort).ToListAsync();
                }

                //制定查询字段 
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                if (sort == null) return await client.Find(filter).Project<T>(projection).ToListAsync();
                //排序查询
                return await client.Find(filter).Sort(sort).Project<T>(projection).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<T>();
            }
        }

        /// <summary>
        /// 分页查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public async Task<List<T>> FindListByPageAsync<T>(FilterDefinition<T> filter, int pageIndex, int pageSize, string[] field = null, SortDefinition<T> sort = null) where T : class, new()
        {
            try
            {
                var client = this.GetCollection<T>();
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return await client.Find(filter).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();
                    //进行排序
                    return await client.Find(filter).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();

                //不排序
                if (sort == null) return await client.Find(filter).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();

                //排序查询
                return await client.Find(filter).Sort(sort).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();

            }
            catch (Exception ex)
            {
                return new List<T>();
            }
        }


        #endregion

    }
}
