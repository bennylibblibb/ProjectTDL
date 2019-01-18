using Ecostar.MQLogger.Core.Infrastructure;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Security.Cryptography;

namespace Ecostar.MQConsumer.Core.Infrastructure
{
    /// <summary>
    ///     ??�u��sender�ϥΨ�
    /// </summary>
    [Export(typeof(IMQContextFactory))]
    public class MQContextFactory : IMQContextFactory
    {
        /// <summary>
        /// �W�U��r��
        /// </summary>
        private static readonly Dictionary<string, MQContext> Contexts = new Dictionary<string, MQContext>();

        /// <summary>
        /// �W�U��ާ@?�r��A�u?�ؤ@��
        /// </summary>
        public static readonly Dictionary<string, object> contextLockers = new Dictionary<string, object>();

        /// <summary>
        /// ��s�W�U��ާ@?�r��?��?�A�u?�ؤ@��
        /// </summary>
        private static readonly object contextLockersLocker = new object();

        /// <summary>
        /// ?�����w���W�U��
        /// </summary>
        /// <param name="mqUri">mq�a�}</param>
        /// <param name="toLog">���??</param>
        /// <returns>�W�U��?�H</returns>
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
                    Console.WriteLine(logHeader + "   ��l��?�e�W�U�姹?");

                    // ?��?��
                    context.SendConnection = CreateConnection(mqUri);
                    context.SendConnection.AutoClose = false;
                    context.SendConnection.ConnectionShutdown += (o, e) => Console.WriteLine("   RabbitMQ??,?���Q??�F�G" + e.ReplyText);
                    Console.WriteLine(logHeader + "   ?��?����?", LogLevel.Trace);

                    // ?���q�D
                    context.SendChannel = CreateChannel(context.SendConnection);
                    Console.WriteLine(logHeader + "   ?�سq�D��?", LogLevel.Trace);

                    Contexts.Add(key, context);

                }

                return context;
            }
        }

        #region �p����k
        /// ?��?��
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
        /// ?�سq�D
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
        /// ?���W�U��ާ@?
        /// </summary>
        /// <param name="contextKey">�W�U��u�Dkey</param>
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
        /// ?���r�Ū�MD5��
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