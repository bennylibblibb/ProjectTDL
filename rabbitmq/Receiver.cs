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
        /// 初始化某???的接收
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

                // ?听Shutdown事件，??下LOG便于排查和?管服?的?定性
                _context.ReceiveConnection.ConnectionShutdown += (o, e) =>
                {
                    _mqActionLogFunc("   RabbitMQ??,?接被??了：" + e.ReplyText);
                };
                // ?取通道
                _context.ReceiveChannel = _context.ReceiveConnection?.CreateModel();

                // ?建事件??的消?者
                var consumer = new EventingBasicConsumer(_context.ReceiveChannel);
                consumer.Received += (o, e) =>
                {
                    try
                    {
                        // 接受?据?理??
                        // e.Body
                        var result = _processFunction(e.Body);

                        if (!isAutoAck)
                        {
                            if (!result)
                            {
                                Thread.Sleep(300);

                                // 未能?理完成的?，?消息重新放入?列?
                                _context.ReceiveChannel.BasicReject(e.DeliveryTag, true);
                                _mqActionLogFunc("   消息未?理成功，?消息重新放入?列?");
                            }
                            else if (!_context.ReceiveChannel.IsClosed)
                            {
                                // ?理成功并且通道未???ack回去，?除?列中的消息
                                _context.ReceiveChannel.BasicAck(e.DeliveryTag, false);
                                _mqActionLogFunc("   消息?理成功,?送Ack完?");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(300);
                        if (!isAutoAck)
                        {
                            // ?消息重新放入?列?
                            _context.ReceiveChannel.BasicReject(e.DeliveryTag, true);
                        }
                        _mqActionLogFunc("   ?理?据?生异常：" + ex.Message + ex.StackTrace);
                    }
                };

                // 一次只?取一?消息
                _context.ReceiveChannel.BasicQos(0, 1, false);
                _context.ReceiveChannel.BasicConsume(_context.ReceiveQueueName, _context.IsAutoAck, consumer);

                _mqActionLogFunc("   初始化?列完?");
            }
            catch (Exception ex)
            {
                _mqActionLogFunc("   初始化RabbitMQ出?：" + ex.Message + ex.StackTrace);
            }
        }

    }
}