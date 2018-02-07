namespace ValleyNet.Network
{

    using UnityEngine.Networking;
    using ValleyNet.Client;


    public class NetClientTransmitter
    {
        public static bool SendOnMain(short msgType, MessageBase msg)
        {
            Client mainInstance = Client.mainInstance;

            if(mainInstance == null)
            {
                return false;
            }
            
            return mainInstance.Send(msgType, msg);
        }
    }
}