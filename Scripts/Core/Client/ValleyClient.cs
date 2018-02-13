/*
*   LLAPI NetworkClient Wrapper for ValleyNet
*   
*   This object generates basic client events based on network data received from Server,
*   synchronizes network/simulation settings with server
*
*/
namespace ValleyNet.Core.Client
{
    using System;
    using UnityEngine;
    using UnityEngine.Networking;
    using ValleyNet.Core.Protocol.Message;
    using ValleyNet.Core.Network;


    public class ValleyClient : IMessageHandlerRegisterable
    {
        protected NetworkClient _client;   // UNET's NetworkClient object
        public static ValleyClient mainInstance; // Main ValleyNet Client instance (first made on machine by default)
        
        /*EVENTS*/
        public event EventHandler ClientRegisteredHandlers;     // Client has registered is basic network handlers
        public event EventHandler ClientConnected;              // Client has connected to a server
        public event EventHandler ClientStartSync;              // Client has initiated synchronizing with the server 
        public event EventHandler ClientFinishSync;             // Client has finished synchronizing with the server
        public event EventHandler ClientAddedPlayer;            // Client has added it's player object to the server
        public event EventHandler ClientDisconnected;           // Client has disconnected from the server
        /********/
        protected string _serverIp = "127.0.0.1"; // Target Server IP
        protected int _serverPort = 8888;         // Target Server Port
        protected string _username = "VN_USER";   // Client's username
        private int _tickRate = 0;               // Update rate of the client's simulation. Syncs with server
        private bool _isConnected = false;
        private ConfigMessage _serverConfig;      // Message containing the server's configuration
        private IdentityMessage _clientIdentity;    // Client Profile container, used to send server our identity

        public string serverIp                   {get{return _serverIp;}}
        public int serverPort                    {get{return _serverPort;}}
        public int tickRate                      {get{return _tickRate;}}
        public bool isConnected                  {get{return _client.isConnected;}}
        public ConfigMessage serverConfig        {get{return _serverConfig;}}
        public NetworkClient uNetClient          {get{return _client;}}


        public ValleyClient(IdentityMessage clientIdentity, bool useBaseHandlers=true)
        {
            _serverConfig = new ConfigMessage();
            _client = new NetworkClient();
            _clientIdentity = clientIdentity;

            if(useBaseHandlers)
            {
                RegisterBaseHandlers();
            }
        }


        // Connect to server as username via LLAPI
        public void Connect(string serverIp="127.0.0.1", int serverPort=8888)
        {
            if(!_isConnected)
            {
                if(_client == null)
                {
                    _client = new NetworkClient(); // init new LLAPI client
                }

                _serverIp = serverIp;
                _serverPort = serverPort;

                _client.Connect(_serverIp, _serverPort); // Connect to LLAPI server via NetworkClient

                Debug.Log("[ValleyNet] Client(IpBind: " + _serverIp + "): Attempting to connect to " + _serverIp + ":" + _serverPort + " as [" + _clientIdentity.username + "]");            
            }
        }


        // Send UNet Message via Client
        public bool Send(short msgType, MessageBase msg)
        {
            if(_isConnected && _client != null)
            {
                return _client.Send(msgType, msg);
            }
            
            return false;
        }


        // Register basic LLAPI Handlers for ValleyClient
        private void RegisterBaseHandlers()
        {
            _client.RegisterHandler(MsgType.Connect, OnClientConnected);
            _client.RegisterHandler(MessageType.ConnectACK, OnClientConnectionACK);
            _client.RegisterHandler(MessageType.Config, OnClientReceivedConfig);
            _client.RegisterHandler(MessageType.AddPlayerACK, OnAddPlayerACK);            
            _client.RegisterHandler(MsgType.Disconnect, OnClientDisconnected);
            
            
            OnClientRegisteredHandlers(); // Raise 'ClientRegisteredHandlers' event
        }


        // Register message handler with NetworkClient
        public void RegisterMessageHandler(short msgType, NetworkMessageDelegate dele)
        {
            _client.RegisterHandler(msgType, dele);
        }


        // Raises 'ClientRegisteredHandlers' Event
        protected virtual void OnClientRegisteredHandlers()
        {
            EventHandler handler = ClientRegisteredHandlers;

            if(handler != null)
            {
                handler(this, new EventArgs());
            }
        }


        // Network Facing, Raises 'ClientConnected' & 'ClientStartSync' Events
        // Sends off client's profile to server
        protected virtual void OnClientConnected(NetworkMessage netMsg)
        {
            ConnectionRequestMessage connREQ = new ConnectionRequestMessage();
            connREQ.username = _username;
            _client.Send(MessageType.ConnectREQ, connREQ);
        }


        // Network Facing, Raises 'ClientDisconnected' Event
        // Resets 'isConnected' to false
        protected virtual void OnClientDisconnected(NetworkMessage netMsg)
        {
            _client.Disconnect();

            // Raise ClientDisconnected Event
            EventHandler handler = ClientDisconnected;
            if(handler != null)
            {
                handler(this, new EventArgs());
            }
        }


        protected virtual void OnClientConnectionACK(NetworkMessage netMsg)
        {
            ConnectionResponseMessage connACK = netMsg.ReadMessage<ConnectionResponseMessage>();

            if(connACK.isAccepted)
            {
                AddPlayerRequestMessage reqMsg = new AddPlayerRequestMessage();
                reqMsg.username = _username;

                // Raise ClientConnected Event
                EventHandler handler = ClientConnected;
                if(handler != null)
                {
                    handler(this, new EventArgs());
                }
                
                _client.Send(MessageType.Identity, _clientIdentity); // Send identity to server
                _client.Send(MessageType.AddPlayerREQ, reqMsg);

                Debug.Log("[ValleyNet] Client(IpBind:" + _serverIp + "): Connection Success! Authenticating profile with server " + _clientIdentity + "...");

                // Raise ClientStartSync Event
                handler = ClientStartSync;
                if(handler != null)
                {
                    handler(this, new EventArgs());
                }
            }
            else if(connACK.maxConnections == connACK.currentConnections)
            {
                Debug.Log("[ValleyNet] Client(IpBind:" + _serverIp + "): Max connection slots full, connection rejected");
            }
            else
            {
                Debug.Log("[ValleyNet] Client(IpBind:" + _serverIp + "): Connection rejected");
            }
        }


        protected virtual void OnAddPlayerACK(NetworkMessage netMsg)
        {
            
        }


        // Network Facing, Raises 'ClientFinishSync' Events
        // Syncs local simulation/network config with remote's simulation/network config
        protected virtual void OnClientReceivedConfig(NetworkMessage netMsg)
        {
            _serverConfig = netMsg.ReadMessage<ConfigMessage>();

            // Sync local simulation settings with remote simulation
            _tickRate = _serverConfig.tickRate; // Set tickrate to server
            Time.fixedDeltaTime = 1/_serverConfig.tickRate; // Set FixedUpdate() rate to server's

            Debug.Log("[ValleyNet] ValleyClient: Server connection permitted [connection# " + _serverConfig.numConnections + "/" + _serverConfig.maxConnections + "]");
            Debug.Log("[ValleyNet] ValleyClient: Synchronized local simulation with server [tickrate: " + _tickRate + "(FixedUpdate() Rate: " + Time.fixedDeltaTime + "ms)]");

            // Raise ClientFinishSync Event
            EventHandler handler = ClientFinishSync;
            if(handler != null)
            {
                handler(this, new EventArgs());
            }
        }


        protected virtual void OnClientAddedPlayer()
        {
            
        }
    }
}