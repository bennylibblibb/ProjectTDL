using RabbitMQ.Client;

namespace Ecostar.MQConsumer.Core
{
    /// <summary>
    ///        MQ ��?��
    /// </summary>
    public partial class MQContext
    {
        // <summary>
        /// ��??�v��Connection
        /// </summary>
        public IConnection ReceiveConnection { get; set; }

        /// <summary>
        /// �Τ_?�v��Channel
        /// </summary>
        public IModel ReceiveChannel { get; set; }

        /// <summary>
        /// ?�v?�C�W
        /// </summary>
        public string ReceiveQueueName { get; set; }
    }
}