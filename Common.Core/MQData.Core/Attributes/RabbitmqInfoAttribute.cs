using System;
using System.Collections.Generic;
using System.Text;

namespace MQData.Core.Attributes
{
  public class RabbitmqInfoAttribute
    {
        /// <summary>
        /// 队列名称
        /// </summary>
        public string QueueName { get; set; }


        /// <summary>
        /// 转换器名称
        /// </summary>
        public string ExchangeName { get; set; }


        public RabbitmqInfoAttribute(string queueName=ConstQueryNmae.DEFALE_PERSONALRUN,string exchangeName=ConstExchangeName.DEFALE_EXCHANGE_NAME)
        {
            this.QueueName = queueName;
            this.ExchangeName = exchangeName;
        }

    }
}
