namespace Ecostar.MQConsumer.Core
{
    public interface IReceiver
    {
        /// <summary>
        ///     ��l�Ʊ����{��
        /// </summary>
        /// <param name="mqUrls"></param>
        void InitialReceive(MQReceiverParam receiverParams);


    }
}