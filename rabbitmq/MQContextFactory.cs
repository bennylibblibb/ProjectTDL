using Ecostar.MQLogger.Core.Infrastructure;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Security.Cryptography;

namespace Ecostar.MQConsumer.Core.Infrastructure
{
    /// <summary>
    ///     ??只有sender使用到
    /// </summary>
    [Export(typeof(IMQContextFactory))]
    public class MQContextFactory : IMQContextFactory
    {
        /// <summary>
        /// 上下文字典
        /// </summary>
        private static readonly Dictionary<string, MQContext> Contexts = new Dictionary<string, MQContext>();

        /// <summary>
        /// 上下文操作?字典，只?建一次
        /// </summary>
        public static readonly Dictionary<string, object> contextLockers = new Dictionary<string, object>();

        /// <summary>
        /// 更新上下文操作?字典?的?，只?建一次
        /// </summary>
        private static readonly object contextLockersLocker = new object();

        /// <summary>
        /// ?取指定的上下文
        /// </summary>
        /// <param name="mqUri">mq地址</param>
        /// <param name="toLog">日志??</param>
        /// <returns>上下文?象</returns>
        public MQContext CreateContext(string mqUri, Action<string, LogLevel> toLog)
        {
            var key = MD5Encrypt(mqUri);
            var locker = GetFactoryLocker(key);

            lock (locker)
            {
                MQContext context;
                if (!Contexts.TryGetValue(key, out context))
                {
                    Guid contextId = Guid.NewGuid();
                    string logHeader = string.Format("[{0}]", contextId.ToString());

                    context = new MQContext()
                    {
                        ReceiveQueueName = "Logs",
                        Id = contextId
                    };
                    Console.WriteLine(logHeader + "   初始化?送上下文完?");

                    // ?取?接
                    context.SendConnection = CreateConnection(mqUri);
                    context.SendConnection.AutoClose = false;
                    context.SendConnection.ConnectionShutdown += (o, e) => Console.WriteLine("   RabbitMQ??,?接被??了：" + e.ReplyText);
                    Console.WriteLine(logHeader + "   ?建?接完?", LogLevel.Trace);

                    // ?取通道
                    context.SendChannel = CreateChannel(context.SendConnection);
                    Console.WriteLine(logHeader + "   ?建通道完?", LogLevel.Trace);

                    Contexts.Add(key, context);

                }

                return context;
            }
        }

        #region 私有方法
        /// ?建?接
        /// </summary>
        /// <param name="mqUrl"></param>
        /// <returns></returns>
        private static IConnection CreateConnection(string mqUrl)
        {
            const ushort heartbeta = 120;

            var factory = new ConnectionFactory()
            {
                Uri = mqUrl,
                RequestedHeartbeat = heartbeta,
                AutomaticRecoveryEnabled = true
            };

            return factory.CreateConnection();
        }

        /// <summary>
        /// ?建通道
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static IModel CreateChannel(IConnection connection)
        {
            if (connection != null)
                return connection.CreateModel();
            return null;
        }


        /// <summary>
        /// ?取上下文操作?
        /// </summary>
        /// <param name="contextKey">上下文工厂key</param>
        /// <returns></returns>
        private static object GetFactoryLocker(string contextKey)
        {
            lock (contextLockersLocker)
            {
                object locker;
                if (!contextLockers.TryGetValue(contextKey, out locker))
                {
                    locker = new object();
                    contextLockers.Add(contextKey, locker);
                }

                return locker;
            }
        }

        /// <summary>
        /// ?取字符的MD5值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string MD5Encrypt(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(str));
            return System.Text.Encoding.Default.GetString(result);
        }
        #endregion


    }
}