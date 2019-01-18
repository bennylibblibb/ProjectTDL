using RabbitMQ.Client;
using System;

namespace Ecostar.MQConsumer.Core
{
    /// <summary>
    ///        MQ 生?者
    /// </summary>
    public partial class MQContext
    {
        /// <summary>
        /// 用于?送消息的Connection
        /// </summary>
        public IConnection SendConnection { get; set; }

        /// <summary>
        /// 用于?送消息到Channel
        /// </summary>
        public IModel SendChannel { get; set; }

        /// <summary>
        /// ?送的Exchange
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// 是否?用自??除
        /// </summary>
        public bool IsAutoAck { get; set; }
        /// <summary>
        /// 上下文ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 路由
        /// </summary>
        public string RouteKey { get; set; }
        /// <summary>
        /// 是否正在?行，默?false
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// 回收此上下文
        /// </summary>
        public void Recovery()
        {
            IsRunning = false;
        }
    }
}