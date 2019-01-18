using RabbitMQ.Client;

namespace Ecostar.MQConsumer.Core
{
    /// <summary>
    ///        MQ 消?者
    /// </summary>
    public partial class MQContext
    {
        // <summary>
        /// 用??听的Connection
        /// </summary>
        public IConnection ReceiveConnection { get; set; }

        /// <summary>
        /// 用于?听的Channel
        /// </summary>
        public IModel ReceiveChannel { get; set; }

        /// <summary>
        /// ?听?列名
        /// </summary>
        public string ReceiveQueueName { get; set; }
    }
}