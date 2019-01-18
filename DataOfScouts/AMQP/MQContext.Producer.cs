using RabbitMQ.Client;
using System;

namespace Ecostar.MQConsumer.Core
{
    /// <summary>
    ///        MQ ��?��
    /// </summary>
    public partial class MQContext
    {
        /// <summary>
        /// �Τ_?�e������Connection
        /// </summary>
        public IConnection SendConnection { get; set; }

        /// <summary>
        /// �Τ_?�e������Channel
        /// </summary>
        public IModel SendChannel { get; set; }

        /// <summary>
        /// ?�e��Exchange
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// �O�_?�Φ�??��
        /// </summary>
        public bool IsAutoAck { get; set; }
        /// <summary>
        /// �W�U��ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string RouteKey { get; set; }
        /// <summary>
        /// �O�_���b?��A�q?false
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// �^�����W�U��
        /// </summary>
        public void Recovery()
        {
            IsRunning = false;
        }
    }
}