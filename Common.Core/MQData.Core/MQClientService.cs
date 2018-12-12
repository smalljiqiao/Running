using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MQData.Core
{
   public class MQClientService:BusBuilder
    {
        IConfigurationRoot _config;
        public MQClientService(IConfigurationRoot config):base(config.GetSection("").Value)
        {
            this._config = config;
        }
        /// <summary>
        /// 创建消息队列
        /// </summary>
        /// <param name="adbus"></param>
        /// <param name="queueName"></param>
        /// <returns></returns>
        private async Task<IQueue> CreateQueue<T>(IAdvancedBus adbus)
        {
            var mqInfo = this.GetMqInfo<T>();
            if (adbus == null)
                return null;
            if (string.IsNullOrEmpty(mqInfo.QueueName))
                return adbus.QueueDeclare();
            return await adbus.QueueDeclareAsync(mqInfo.QueueName);
        }

        #region Fanout

        /// <summary>
        /// 消息消耗
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="handler">回调</param>
        /// <param name="exChangeName">交换器名称</param>
        /// <param name="queueName">队列名</param>
        /// <param name="routingKey">路由</param>
        public async void FanoutConsume<T>(Action<T> handler, string routingKey = "") where T : class
        {
            var bus = this.CreateMessageBus();
            var adbus = bus.Advanced;
            var mqInfo = this.GetMqInfo<T>();
            var exchange = adbus.ExchangeDeclare(mqInfo.ExchangeName, ExchangeType.Fanout);
            var queue = await this.CreateQueue<T>(adbus);
            adbus.Bind(exchange, queue, routingKey);
            adbus.Consume(queue, registration =>
            {
                registration.Add<T>((messsage, info) =>
                {
                    handler(messsage.Body);
                });
            });
        }

        /// <summary>
        /// 消息上报
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="topic">主题名</param>
        /// <param name="t">消息命名</param>
        /// <param name="msg">错误信息</param>
        /// <returns></returns>
        public bool FanoutPush<T>(T t, out string msg, string routingKey = "") where T : class
        {
            msg = string.Empty;
            try
            {
                var mqInfo = this.GetMqInfo<T>();
                using (var bus = this.CreateMessageBus())
                {
                    var adbus = bus.Advanced;
                    var exchange = adbus.ExchangeDeclare(mqInfo.ExchangeName, ExchangeType.Fanout);
                    adbus.Publish(exchange, routingKey, false, new Message<T>(t));
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        #endregion

        #region Direct 

        /// <summary>
        /// 消息发送
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="queue">队列名</param>
        /// <param name="message">消息数据</param>
        public void DirectSend<T>(T message, string queueName = null) where T : class
        {
            if (queueName == null)
            {
                var mqInfo = this.GetMqInfo<T>();
                if (mqInfo != null)
                {
                    queueName = mqInfo.QueueName;
                }
            }

            if (string.IsNullOrEmpty(queueName)) { return; }

            using (var bus = this.CreateMessageBus())
            {
                bus.Send(queueName, message);
            }
        }
       
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="queue">队列</param>
        /// <param name="callback">回调</param>
        /// <param name="msg">接收信息</param>
        public bool DirectReceive<T>(Action<T> callback, out string msg) where T : class
        {
            msg = string.Empty;
            var mqInfo = this.GetMqInfo<T>();
            try
            {
                var bus = this.CreateMessageBus();
                bus.Receive<T>(mqInfo.QueueName, callback);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 消息发送
        /// <![CDATA[（direct EasyNetQ高级API）]]>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        /// <param name="exChangeName"></param>
        /// <param name="routingKey"></param>
        /// <returns></returns>
        public bool DirectPush<T>(T t, out string msg, string routingKey = "direct_rout_default") where T : class
        {
            msg = string.Empty;
            try
            {
                var mqInfo = this.GetMqInfo<T>();
                using (var bus = this.CreateMessageBus())
                {
                    var adbus = bus.Advanced;
                    var exchange = adbus.ExchangeDeclare(mqInfo.ExchangeName, ExchangeType.Direct);
                    adbus.Publish<T>(exchange, routingKey, false, new Message<T>(t));
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 消息接收
        ///  <![CDATA[（direct EasyNetQ高级API）]]>
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="handler">回调</param>
        /// <param name="exChangeName">交换器名</param>
        /// <param name="queueName">队列名</param>
        /// <param name="routingKey">路由名</param>
        public bool DirectConsuem<T>(Action<T> handler, out string msg, string routingKey = "direct_rout_default")
            where T : class
        {
            msg = string.Empty;
            try
            {
                using (var bus = this.CreateMessageBus())
                {
                    var mqInfo = this.GetMqInfo<T>();
                    var adbus = bus.Advanced;
                    var exchange = adbus.ExchangeDeclare(mqInfo.ExchangeName, ExchangeType.Direct);
                    var queue = this.CreateQueue<T>(adbus).Result;
                    adbus.Bind(exchange, queue, routingKey);
                    adbus.Consume(queue, registration =>
                    {
                        registration.Add<T>((message, info) =>
                        {
                            handler(message.Body);
                        });
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }
        #endregion
    }
}
