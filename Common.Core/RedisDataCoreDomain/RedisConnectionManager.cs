using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
namespace RedisDataCoreDomain
{
    public class RedisConnectionManager
    {
        private static ConnectionMultiplexer _instance;
        private static readonly object lock_obj = new object();
        /// <summary>
        /// 名称的前缀名
        /// </summary>
        public static string SysPrefix = "";

        private static string RedisConnectionString = "";
        public static ConnectionMultiplexer Instance {
            get {
                if (_instance == null) {
                    lock (lock_obj) {
                        if (_instance == null) {
                            var configBasePath = Directory.GetCurrentDirectory();
                            var builder=new ConfigurationBuilder().SetBasePath(configBasePath).AddJsonFile("redisConfig.json");
                            var config = builder.Build();
                            SysPrefix = config.GetSection("RedisConfig.sysPrefix").Value;
                            RedisConnectionString = config.GetSection("RedisConfig.redisConnectionString").Value;
                            GetManager();
                        }
                    }
                }

              return _instance;
            }
        }
       
        /// <summary>
        /// 获取redis链接管理
        /// </summary>
        /// <returns></returns>
        public static ConnectionMultiplexer GetManager(string connectionString = null)
        {
            connectionString = connectionString ?? RedisConnectionString;
            var connect = ConnectionMultiplexer.Connect(connectionString);
            //注册连接失败事件
            connect.ConnectionFailed += MuxerConnectionFailed;

            //注册连接重置事件
            connect.ConnectionRestored += MuxerConnectionRestored;

            //注册错误事件
            connect.ErrorMessage += MuxerErrorMessage;

            //注册配置更改事件
            connect.ConfigurationChanged += MuxerConfigurationChanged;

            //集群更改事件
            connect.HashSlotMoved += MuxerHashSlotMoved;

            //注册类库错误事件
            connect.InternalError += MuxerInternalError;

            return connect;
        }
        #region 事件

        /// <summary>
        /// 更改配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs args)
        {
            Console.WriteLine(string.Format("配置更改了:{0}", args.EndPoint));
        }

        /// <summary>
        /// 发生错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs args)
        {
            Console.WriteLine(string.Format("{0} 错误: endpoint:{0},msg:{1}", DateTime.Now, args.EndPoint, args.Message));
        }

        /// <summary>
        /// 链接被重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs args)
        {
            Console.WriteLine(string.Format("{0} 连接重置 endpoint:{0},msg:{1}", DateTime.Now, args.EndPoint, args.ConnectionType.GetEnumDesc()));
        }

        /// <summary>
        /// 链接失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs args)
        {
            Console.WriteLine(string.Format("{0} 连接失败：endpoint:{0},msg:{1}", DateTime.Now, args.EndPoint, args.ConnectionType.GetEnumDesc()));
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Console.WriteLine("更改集群:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Console.WriteLine("redis类库错误:Message" + e.Exception.Message);
        }

        #endregion

    }
}
