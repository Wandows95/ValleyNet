namespace ValleyNet.Core.Network
{
    using UnityEngine.Networking;


    public interface IMessageHandlerRegisterable
    {
        void RegisterMessageHandler(short msgType, NetworkMessageDelegate dele);    
    }
}