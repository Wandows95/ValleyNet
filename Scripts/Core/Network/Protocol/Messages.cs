namespace ValleyNet.Core.Protocol.Message
{
    using UnityEngine;
    using UnityEngine.Networking;

    // Base player profile message
    public class ProfileMessage : MessageBase
    {
        public string username = "";
        public int userId = 0;

        public override string ToString()
        {
            return "[" + username + "]";
        }
    }


    // Base Networked Instance config message
    // Generally originating from the server
    public class ConfigMessage : MessageBase
    {
        public int tickRate = 0;
        public int numConnections = 0;
        public int maxConnections = 0;
        public string MOTD = "";
        public string serverName = "";

        public override string ToString()
        {
            return serverName + "[" + tickRate + "tick ]";
        }
    }


    public class AddPlayerRequestMessage : MessageBase
    {
        
    }


    public class MessageType
    {
        public const short Profile = 48; // Unity's MsgType.Highest + 1
        public const short Config = 49;
        public const short AddPlayerReq = 50;
    }
}