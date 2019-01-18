using Ecostar.MQLogger.Core.Infrastructure;
using System;

namespace Ecostar.MQConsumer.Core.Infrastructure
{
    /// <summary>
    ///     ??�u��sender�ϥΨ�
    /// </summary>
    public interface IMQContextFactory
    {
        MQContext CreateContext(string mqUri, Action<string, LogLevel> toLog);
    }
}