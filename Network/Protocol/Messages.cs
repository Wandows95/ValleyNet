namespace ValleyNet.Protocol.Message
{
    using UnityEngine;
    using UnityEngine.Networking;

    public class ProfileMessage : MessageBase
    {
        public string username = "";
        public int userId = 0;

        public override string ToString()
        {
            return "[" + username + "]";
        }
    }

    public class MessageType
    {
        public const short Profile = 48; // Unity's MsgType.Highest + 1
    }
}