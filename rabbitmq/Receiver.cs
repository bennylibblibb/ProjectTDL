using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;

namespace Ecostar.MQConsumer.Core
{
    [Export(typeof(IReceiver))]
    public class Receiver : IReceiver
    {
        private MQContext _context;
        private const ushort Heartbeta = 60;
        private string _queueName;
        private bool _isAutoAck;
        private List<string> _mqUrls;
        private Func<byte[], bool> _processFunction;
        private Action<string> _mqActionLogFunc;
        private MQConnectionFactory _ConnectionFactoryParams;

   
        public void InitialReceive(MQReceiverParam receiverParams)
        {
            _queueName       = receiverParams._queueName;
            _isAutoAck       = receiverParams._isAutoAck;
            _mqUrls          = receiverParams._mqUrls;
            _processFunction = receiverParams._processFunction;
            _mqActionLogFunc = receiverParams._mqActionLogFunc;
            _ConnectionFactoryParams = receiverParams.ConnectionFactoryParam;
            receiverParams._mqUrls.ForEach(url => InitReceive(_queueName, _isAutoAck, url));

        }

        /// <summary>
        /// ��l�ƬY???������
        /// </summary>
        private void InitReceive(string queueName, bool isAutoAck, string mqUrl)
        {
            Guid contextId = Guid.NewGuid();
            string logHeader = string.Format("[{0}, {1}]", queueName, contextId.ToString());
            try
            {
                _context = new MQContext()
                {
                    Id = contextId,
                    ReceiveQueueName = queueName,
                    IsAutoAck = isAutoAck,
                    ReceiveConnection = new ConnectionFactory()
                    {
                        HostName = _ConnectionFactoryParams.HostName,
                        UserName = _ConnectionFactoryParams.UserName,
                        Password = _ConnectionFactoryParams.Password,
                        VirtualHost = _ConnectionFactoryParams.VirtualHost
                    }.CreateConnection()
                };

                // ?�vShutdown�ƥ�A??�ULOG�K�_�Ƭd�M?�ުA?��?�w��
                _context.ReceiveConnection.ConnectionShutdown += (o, e) =>
                {
                    _mqActionLogFunc("   RabbitMQ??,?���Q??�F�G" + e.ReplyText);
                };
                // ?���q�D
                _context.ReceiveChannel = _context.ReceiveConnection?.CreateModel();

                // ?�بƥ�??����?��
                var consumer = new EventingBasicConsumer(_context.ReceiveChannel);
                consumer.Received += (o, e) =>
                {
                    try
                    {
                        // ����?�u?�z??
                        // e.Body
                        var result = _processFunction(e.Body);

                        if (!isAutoAck)
                        {
                            if (!result)
                            {
                                Thread.Sleep(300);

                                // ����?�z������?�A?�������s��J?�C?
                                _context.ReceiveChannel.BasicReject(e.DeliveryTag, true);
                                _mqActionLogFunc("   ������?�z���\�A?�������s��J?�C?");
                            }
                            else if (!_context.ReceiveChannel.IsClosed)
                            {
                                // ?�z���\�}�B�q�D��???ack�^�h�A?��?�C��������
                                _context.ReceiveChannel.BasicAck(e.DeliveryTag, false);
                                _mqActionLogFunc("   ����?�z���\,?�eAck��?");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(300);
                        if (!isAutoAck)
                        {
                            // ?�������s��J?�C?
                            _context.ReceiveChannel.BasicReject(e.DeliveryTag, true);
                        }
                        _mqActionLogFunc("   ?�z?�u?���ݱ`�G" + ex.Message + ex.StackTrace);
                    }
                };

                // �@���u?���@?����
                _context.ReceiveChannel.BasicQos(0, 1, false);
                _context.ReceiveChannel.BasicConsume(_context.ReceiveQueueName, _context.IsAutoAck, consumer);

                _mqActionLogFunc("   ��l��?�C��?");
            }
            catch (Exception ex)
            {
                _mqActionLogFunc("   ��l��RabbitMQ�X?�G" + ex.Message + ex.StackTrace);
            }
        }

    }
}