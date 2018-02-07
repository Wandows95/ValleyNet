namespace ValleyNet.Network
{

    using UnityEngine.Networking;
    using ValleyNet.Core.Client;


    public class NetClientTransmitter
    {
        public static bool SendOnMain(short msgType, MessageBase msg)
        {
            ValleyClient mainInstance = ValleyClient.mainInstance;

            if(mainInstance == null)
            {
                return false;
            }
            
            return mainInstance.Send(msgType, msg);
        }
    }
}