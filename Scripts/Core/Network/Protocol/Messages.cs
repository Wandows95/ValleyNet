namespace ValleyNet.Core.Protocol.Message
{
    using UnityEngine;
    using UnityEngine.Networking;
    using ValleyNet.Core.Tag;


    // Base player profile message
    public class IdentityMessage : MessageBase
    {
        // PAYLOAD SIZE: 32 + n bits (4 + n/8 bytes)
        public string username = "";        // n bits(n/8 bytes)
        public int userId = 0;              // 32 bits(4 bytes)

        public override string ToString()
        {
            return "[" + username + "]";
        }
    }


    // Base Networked Instance config message
    // Generally originating from the server
    public class ConfigMessage : MessageBase
    {
        // PAYLOAD SIZE: 96 + p + q bits (12 + p/8 + q/8 bytes)
        public int tickRate = 0;        // 32 bits (4 bytes)
        public int numConnections = 0;  // 32 bits (4 bytes)
        public int maxConnections = 0;  // 32 bits (4 bytes)
        public string MOTD = "";        // p bits (p/8 bytes)
        public string serverName = "";  // q bits (q/8 bytes)

        public ConfigMessage(int tickRate, int numConnections, int maxConnections, string serverName, string MOTD)
        {
            this.tickRate = tickRate;
            this.numConnections = numConnections;
            this.maxConnections = maxConnections;
            this.serverName = serverName;
            this.MOTD = MOTD;
        }


        public override string ToString()
        {
            return serverName + "[" + tickRate + "tick ]";
        }
    }


    public class ConnectionRequestMessage : MessageBase
    {
        public string username;

        public override string ToString()
        {
            return "Connection REQ: " + username;
        }
    }

    
    public class ConnectionResponseMessage : MessageBase
    {
        public bool isAccepted;        // Was the connection accepted?
        public int maxConnections;     // Max connections allowed by server
        public int currentConnections; // Current number of connections allowed on server
        public byte permissionLevel;   // If accepted, what level was granted?
    }


    public class IdentityResponseMessage : MessageBase
    {
        public bool isAccepted;        // Was the identity accepted?
        public byte permissionLevel;   // If accepted, what level was granted?

        public IdentityResponseMessage(bool isAccepted, byte permissionLevel)
        {
            this.isAccepted = isAccepted;
            this.permissionLevel = permissionLevel;
        }
    }


    public class AddPlayerRequestMessage : MessageBase
    {
        public string username;
        public byte reqCode;       // Additional encoded data about player req (Team, Faction)
    }


    public class AddPlayerResponseMessage : MessageBase
    {
        public bool isAccepted;    // Was the player addition accepted?
        public int maxPlayers;     // Max Players allowed by server
        public int currentPlayers; // Current number of players allowed on server
        public byte ackCode;       // Additional encoded data about player addition
    }

    
    public class MessageType
    {
        public const short Identity = 48; // Unity's MsgType.Highest + 1
        public const short Config = 49;
        public const short ConnectREQ = 50;
        public const short ConnectACK = 51;
        public const short AddPlayerREQ = 52;
        public const short AddPlayerACK = 53;
        public const short IdentityResponse = 54;
    }
}