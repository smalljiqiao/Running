using EasyNetQ;
using MQData.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MQData.Core
{
  public class BusBuilder
    {
        private string _conectionString { get; set; }

        public BusBuilder(string connectionString)
        {
            _conectionString = connectionString;

        }
        public IBus CreateMessageBus() {
            if (string.IsNullOrEmpty(_conectionString)) {
                throw new Exception("消息列队字符串为空");
            }
            return RabbitHutch.CreateBus(_conectionString);

        }
        /// <summary>
        /// 从类的属性中-获取消息队列信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public RabbitmqInfoAttribute GetMqInfo<T>(bool isThrow = true)
        {
            var type = typeof(T);
            var attrs = type.GetCustomAttributes(typeof(RabbitmqInfoAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                return attrs[0] as RabbitmqInfoAttribute;
            }

            if (isThrow) { throw new Exception($"{typeof(T).ToString()}没有添加RabbitmqInfoAttribute特性"); }
            return null;
        }
    }
}
