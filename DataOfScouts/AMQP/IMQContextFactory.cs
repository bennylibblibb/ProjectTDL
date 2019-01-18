//using Ecostar.MQLogger.Core.Infrastructure;
using System;

namespace Ecostar.MQConsumer.Core.Infrastructure
{
    /// <summary>
    ///     ??只有sender使用到
    /// </summary>
    public interface IMQContextFactory
    {
        MQContext CreateContext(System.Uri mqUri, Action<string, bool> toLog);
    }
}