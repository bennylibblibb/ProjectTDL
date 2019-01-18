using System;
using System.Collections.Generic;

namespace Ecostar.MQConsumer.Core
{
    /// <summary>
    ///     消?者入?
    /// </summary>
    public class MQReceiverParam
    {
        public  string _queueName { get; set; }
        public  bool _isAutoAck { get; set; }
        public  List<string> _mqUrls { get; set; }
        public  Func<byte[], bool> _processFunction { get; set; }
        public  Action<string> _mqActionLogFunc { get; set; }
        public MQConnectionFactory ConnectionFactoryParam { get; set; }

    }

    /// <summary>
    ///     服?配置
    /// </summary>
    public class MQConnectionFactory
    {
        public string HostName     {get;set;}
        public string UserName     {get;set;}
        public string Password     {get;set;}
        public string VirtualHost  {get;set;}
    }

}