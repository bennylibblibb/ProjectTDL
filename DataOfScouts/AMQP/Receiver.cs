using DataOfScouts;
using FileLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
//using System.ComponentModel.Composition;
using System.Threading;

namespace Ecostar.MQConsumer.Core
{
    //  [Export(typeof(IReceiver))]
    public class Receiver : IReceiver
    {
        private MQContext _context;
        private const ushort Heartbeta = 60;
        private string _queueName;
        private bool _isAutoAck;
        private List<System.Uri> _mqUrls;
        // private Func<byte[], bool> _processFunction;
        private Func<string, bool> _processFunction;
        private Action<string> _mqActionLogFunc;
        private MQConnectionFactory _ConnectionFactoryParams;
        private int count = 0;

        public void InitialReceive(MQReceiverParam receiverParams)
        {
            _queueName = receiverParams._queueName;
            _isAutoAck = receiverParams._isAutoAck;
            _mqUrls = receiverParams._mqUrls;
            _processFunction = receiverParams._processFunction;
            _mqActionLogFunc = receiverParams._mqActionLogFunc;
            _ConnectionFactoryParams = receiverParams.ConnectionFactoryParam;
            receiverParams._mqUrls.ForEach(url => InitReceive(_queueName, _isAutoAck, url));

        }

        /// <summary>
        /// 初始化某???的接收
        /// </summary>
        private void InitReceive(string queueName, bool isAutoAck, System.Uri mqUrl)
        {
            Guid contextId = Guid.NewGuid();
            /// string logHeader = string.Format("[{0}, {1}]", queueName, contextId.ToString());
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
                        VirtualHost = _ConnectionFactoryParams.VirtualHost,
                        Port = _ConnectionFactoryParams.Port
                    }.CreateConnection()
                };

                // ?听Shutdown事件，??下LOG便于排查和?管服?的?定性
                _context.ReceiveConnection.ConnectionShutdown += (o, e) =>
                {
                    // _mqActionLogFunc("RabbitMQ error,connection is closed! " + e.ReplyText);
                    Files.WriteError("RabbitMQ error,connection is closed! " + e.ReplyText);
                };
                // ?取通道
                _context.ReceiveChannel = _context.ReceiveConnection?.CreateModel();
                //  if (AppFlag.TestMode) _processFunction(" Send QOS                              .");

                // Set enqueue one by one msg
                _context.ReceiveChannel.BasicQos(prefetchSize: 0, prefetchCount: AppFlag.AmqpRefetchCount, global: false);

                //   if (AppFlag.TestMode) _processFunction(" Sent QOS                               .");
                // ?建事件??的消?者
                var consumer = new EventingBasicConsumer(_context.ReceiveChannel);

                QueueDeclareOk resultB = _context.ReceiveChannel.QueueDeclarePassive("telecom-digital-data-limited");
                uint countA = resultB != null ? resultB.MessageCount : 0;
                _mqActionLogFunc("====AMQP " + countA.ToString() + " ====");
                if (AppFlag.TestMode) Files.WriteTestLog("Queue", "====AMQP " + countA.ToString() + " ====");
                Files.WriteLog("====AMQP " + countA.ToString() + " ====");
                consumer.Received += (o, e) =>
                {
                    //new Thread(() =>
                    //{
                    try
                    {
                        //   new Thread(() =>
                        //{
                        resultB = _context.ReceiveChannel.QueueDeclarePassive("telecom-digital-data-limited");
                        countA = resultB != null ? resultB.MessageCount : 0;

                        count++;
                        // if (AppFlag.TestMode) Files.WriteTestLog("Queue", countA + "/" + count.ToString() + "  Start.");

                       // if(count ==40) throw (new Exception ("for testing "));
                        // 接受?据?理?? e.Body
                        string message = Encoding.UTF8.GetString(e.Body);
                        var result = true;
                        ////if (message.IndexOf("\"sport_id\":5")>0)
                        ////{
                        ////    if (AppFlag.TestMode) Files.WriteTestLog("Queue", countA + "/" + count.ToString() + " " + (message.Length > 63 ? message.Substring(0, 63) + "     " : message + "      soprtsid =5       ---") + e.DeliveryTag + "  soprtsid =5");
                        ////    result = _processFunction(message);
                        ////} 
                        if (AppFlag.TestMode) Files.WriteTestLog("Queue", "Received1: " + countA + "/" + count.ToString() + " " + (message.Length > 17 ? message.Substring(0, 17) + "   " : message) + e.DeliveryTag);
                        result = _processFunction(message);
                        //else
                        //{
                        //    if (AppFlag.TestMode) Files.WriteTestLog("Queue", countA + "/" + count.ToString() + " " + (message.Length > 63 ? message.Substring(0, 63) + "     " : message + "        soprtsid !=5        ---") + e.DeliveryTag + "  soprtsid !=5");
                        //    result = true;
                        //}
                        if (!isAutoAck)
                        {
                            if (!result)
                            {
                                Thread.Sleep(10);
                                if (AppFlag.TestMode) Files.WriteTestLog("amqplog", "message=" + message + "\r\n");
                                // 未能?理完成的?，?消息重新放入?列?
                                //// _context.ReceiveChannel.BasicReject(e.DeliveryTag, true);
                                ////  _mqActionLogFunc("Message rejet to queues!");
                            }
                            else if (!_context.ReceiveChannel.IsClosed)
                            {
                                // ?理成功并且通道未???ack回去，?除?列中的消息
                                _context.ReceiveChannel.BasicAck(e.DeliveryTag, false);
                                // if (AppFlag.TestMode) _processFunction("Sent Ack!          ---"+ e.DeliveryTag);
                                ///  if (AppFlag.TestMode) Files.WriteTestLog("Queue", countA + "/" + count.ToString() + " " + "Sent Ack          ---" + e.DeliveryTag);
                            }
                        }
                        //})
                        //   { IsBackground = true }.Start();
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(10);
                        if (!isAutoAck)
                        {
                            if (AppFlag.TestMode) Files.WriteTestLog("amqplog", "e.Body=" + e.Body + "\r\n");
                            //// ?消息重新放入?列?
                            //// _context.ReceiveChannel.BasicReject(e.DeliveryTag, true);
                            ////_mqActionLogFunc("Message rejet to queues!");
                            _context.ReceiveChannel.BasicReject(e.DeliveryTag, false); //20191113 remove exp message from queues
                        }

                        // _mqActionLogFunc("RabbitMQ error, " + ex.Message+ex.StackTrace );
                        Files.WriteError("RabbitMQ error, " + ex.Message + ex.StackTrace);
                    }
                    //})
                    //{ IsBackground =true }.Start();
                };

                // 一次只?取一?消息
                //_context.ReceiveChannel.BasicQos(0,1, false);
                _context.ReceiveChannel.BasicConsume(_context.ReceiveQueueName, _context.IsAutoAck, consumer);

                //  _mqActionLogFunc("Initialled AMQP.");
                Files.WriteLog("Initialled AMQP.");
            }
            catch (Exception ex)
            {
                // _mqActionLogFunc("Initial queues error," + ex.Message + ex.StackTrace); 
                Files.WriteError("Initial AMQP error," + ex.Message + ex.StackTrace);
                //throw (ex);
            }
        }

    }
}