//using Ecostar.MQLogger.Core.Infrastructure;
using System;

namespace Ecostar.MQConsumer.Core.Infrastructure
{
    /// <summary>
    ///     ??�u��sender�ϥΨ�
    /// </summary>
    public interface IMQContextFactory
    {
        MQContext CreateContext(System.Uri mqUri, Action<string, bool> toLog);
    }
}