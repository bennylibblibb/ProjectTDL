namespace Ecostar.MQConsumer.Core
{
    public interface IReceiver
    {
        /// <summary>
        ///     初始化接收程序
        /// </summary>
        /// <param name="mqUrls"></param>
        void InitialReceive(MQReceiverParam receiverParams);


    }
}