///<file>
///<summary>
///LLAPI NetworkServer Wrapper for ValleyNet
///   
///This object essentially acts as a base host configuration object in addition
///to generating C# events based on network events
///</summary>
///<remarks>
///Default Handshake:
/// Implicit Connect Provided by UNET NetworkServer on MsgType.Connect
/// NetRaiseConnectionRequested(connREQ) "Valley Connect REQ" -> ConnectionACK
/// NetRaiseClientIdentified(clientId) "Client has Id'd itself" -> serverConfig || idACK
/// NetRaiseClientRequestedAddPlayer(AddPlayerREQ) "Client wants to add player" -> AddPlayerACK
///</remarks>
///</file>
// Disable P2P Host Migration in UNET Layer
#undef ENABLE_UNET_HOST_MIGRATION

namespace ValleyNet.Core.Server
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;
    using ValleyNet.Core.Protocol.Message;
    using ValleyNet.Core.Tag;
    using ValleyNet.Core.Session;
    using ValleyNet.Core.Server.Event;
    using ValleyNet.Core.Network;

    // What phase of execution the server is currently in
    // Server state supercedes Gamemode's phase scheduler (Gamemode controls server state while server is in "Play" state via Phases)
    // That being said, server-level code can forcibly end the "Play" state from outside a Gamemode
    public class ServerState
    {
        public const short Setup = 1;   // Server is setting up or loading gamemode
        public const short Lobby = 2;   // Server is idling, waiting for new connections (dictated by gamemode scheduler)
        public const short Play = 3;    // Server is in main game loop (dictated by gamemode scheduler)
        public const short Pause = 4;   // Server is in suspended state, sending idle frames to all clients and freezing simulation
    }


    // Server config container
    public class ServerConfig
    {
        public string serverName = "";
        public int serverPort = 8888;
        public int maxConnections = 10;
        public string serverMOTD = "Powered by ValleyNet";
        public List<string> bannedIP = new List<string>(); 
        public List<string> whitelistedIP = new List<string>();
        public List<string> whitelistedUsers = new List<string>();
        public bool runInBackground = true;

        public int tickrate = 64; // Ticks per second
    }


    public class ValleyServer : IMessageHandlerRegisterable
    {
        private volatile static ValleyServer _instance;
        private volatile static NetworkServer _server;

        public static ValleyServer Instance {
            get{ 
                if(_instance != null){ return _instance; } 
                else{ new ValleyServer(); return _instance; }
            }
        }
        
        /* STATIC EVENTS */
        public static event EventHandler                    ServerCreated;            // Raised when a ValleyServer is created
        /* EVENTS */
        public event EventHandler                           ServerStartedListening;
        public event EventHandler<SessionEventArgs>         ServerAddedMainSession;   // Most games will only need the main session
        public event EventHandler<SessionEventArgs>         ServerAddedNewSession;    // Raised if server adds a NON-MAIN session (multiple game sessions on one server)
        public event EventHandler<SessionEventArgs>         ServerRemovedSession;     // Raised if server removes a NON-MAIN session (a main session exists until end of server)
        public event EventHandler<ConnectionEventArgs>      ServerConnectedPlayer;
        public event EventHandler<NetworkMessageEventArgs>  ClientRequestedAddPlayer;
        public event EventHandler<NetworkMessageEventArgs>  ServerReceivedProfile;    // Raised when server receives a client's identity profile
        public event EventHandler<NetworkMessageEventArgs>  ServerRequestedAddPlayer;
        public event EventHandler<ConnectionEventArgs>      ServerDisconnectedPlayer;
        /**********/
        protected short _serverState;              // Current state in the server lifecycle
        protected Dictionary<string, List<Session>> _activeSessions; // Key: Tag, Value: List of sessions
        protected List<IdentityTag> _connectedClients;
        protected ServerConfig _config;
        protected HostTopology _topology; // UNET Network Topology to use

        public short serverState {get{return _serverState;}}
        public ServerConfig config {get{return _config;}}

        internal ValleyServer()
        {
            _serverState = ServerState.Setup;
            _activeSessions = new Dictionary<string, List<Session>>();
            _connectedClients = new List<IdentityTag>();
            
            if(_instance == null)
            {
                _instance = this;
            }
        }

        // Invokes NetworkServer.Listen and shifts the serverState to LOBBY
        public void Listen(ServerConfig config, int serverPort, HostTopology topology=null)
        {
            _topology = topology;
            _config = config;
            BindBaseNetworkHandlers();

            if(!NetworkServer.active)
            {
                Application.runInBackground = true;
                NetworkServer.Listen(serverPort);
                _serverState = ServerState.Lobby;
                OnServerStartedListening();
                Debug.Log("[ValleyNet] Server \'" + _config.serverName + "\' listening on " + _config.serverPort);
            }
        }

        public virtual void RegisterSession(Session session)
        {
            List<Session> val;

            // If Session tag has not been registered
            if(!_activeSessions.TryGetValue(session.tag, out val))
            {
                _activeSessions.Add(session.tag, new List<Session>()); // Register new tag
            }
            else
            {
                val.Add(session); // Add session to activeSessions
            }
        }

        public virtual List<Session> GetActiveSessions(string tag)
        {
            List<Session> val;

            // If session tag has not been registered
            if(!_activeSessions.TryGetValue(tag, out val))
            {
                return new List<Session>();
            }

            return val;
        }

        // Register message handler with NetworkServer
        public void RegisterMessageHandler(short msgType, NetworkMessageDelegate dele)
        {
            NetworkServer.RegisterHandler(msgType, dele);
        }

        // Register Handlers for basic server functionality
        private void BindBaseNetworkHandlers()
        {
            NetworkServer.RegisterHandler(MessageType.ConnectREQ, NetRaiseConnectionRequested); // Client requested a connection
            NetworkServer.RegisterHandler(MessageType.Identity, NetRaiseClientIdentified);       // Client sent it's identity
            NetworkServer.RegisterHandler(MessageType.AddPlayerREQ, NetRaiseClientRequestedAddPlayer); // Client requested at add it's player entity
            NetworkServer.RegisterHandler(MsgType.Disconnect, NetRaiseServerDisconnectedPlayer); // Client disconnects
        }

        // Raises static 'ServerCreated' Event
        protected static void OnServerCreated()
        {
            EventHandler handler = ServerCreated;

            if(handler != null)
            {
                handler(typeof(ValleyServer), new EventArgs());
            }
        }

        // Raises 'ServerStartedListening' Event
        protected virtual void OnServerStartedListening()
        {
            EventHandler handler = ServerStartedListening;

            if(handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        // Raises 'ServerConnectedPlayer' Event
        protected virtual void OnServerConnectedPlayer(ConnectionEventArgs eventArgs)
        {   
            EventHandler<ConnectionEventArgs> handler = ServerConnectedPlayer;
            
            if(handler != null)
            {
                handler(this, eventArgs);
            }
        }

        protected virtual void OnClientIdentified(IdentityTag t)
        {
            _connectedClients.Add(t);
        }

        protected virtual void OnClientRequestedAddPlayer(NetworkMessage netMsg)
        {
            Debug.Log("[ValleyNet] Server: Request to add player received");

            EventHandler<NetworkMessageEventArgs> handler = ClientRequestedAddPlayer;
            if(handler != null)
            {
                handler(this, new NetworkMessageEventArgs(netMsg));
            }
        }

        // Raises 'ServerDisconnectedPlayer' Event
        protected virtual void OnServerDisconnectedPlayer(ConnectionEventArgs eventArgs)
        {
            EventHandler<ConnectionEventArgs> handler = ServerDisconnectedPlayer;
            
            if(handler != null)
            {
                handler(this, eventArgs);
            }
        }

        // <--- ConnectREQ
        private void NetRaiseConnectionRequested(NetworkMessage netMsg)
        {
            OnServerConnectedPlayer(new ConnectionEventArgs(netMsg.conn));
            /* 
            ConnectionResponseMessage newResponse = new ConnectionResponseMessage();
            newResponse.isAccepted = true;
            newResponse.maxConnections = _config.maxConnections;
            newResponse.currentConnections = NetworkServer.connections.Count;
            */

            Debug.Log("[ValleyNet] Server received new Connection Request [src: " + netMsg.conn.address  + "(" + netMsg.conn.connectionId + "), accepted: " + true + "]");
            //NetworkServer.SendToClient(netMsg.conn.connectionId, MessageType.ConnectACK, newResponse); // ---> ConnectACK
            
            ConfigMessage cfgMsg = new ConfigMessage(_config.tickrate, NetworkServer.connections.Count, _config.maxConnections, _config.serverName, _config.serverMOTD);
            NetworkServer.SendToClient(netMsg.conn.connectionId, MessageType.Config, cfgMsg);
        }

        // Network Facing, Raises 'ServerIdentitySync' Event
        private void NetRaiseClientIdentified(NetworkMessage netMsg)
        {
            IdentityMessage identityMessage = netMsg.ReadMessage<IdentityMessage>();

            Debug.Log("[ValleyNet] Server received new identity [src: " + netMsg.conn.address  + "(" + netMsg.conn.connectionId + ")] Username: " + identityMessage.username);            
            OnClientIdentified(new IdentityTag(netMsg.conn, Permissions.PLAYER, identityMessage.username));

            IdentityResponseMessage responseMessage = new IdentityResponseMessage(true, (byte)Permissions.PLAYER);
            NetworkServer.SendToClient(netMsg.conn.connectionId, MessageType.IdentityResponse, responseMessage);
            // TO-DO Invoke ServerProfileSync, spawn playertag, register new player
        }

        // <--- PlayerAddREQ
        private void NetRaiseClientRequestedAddPlayer(NetworkMessage netMsg)
        {
            OnClientRequestedAddPlayer(netMsg);
        }

        // Network Facing, Raises 'ServerDisconnectedPlayer' Event
        private void NetRaiseServerDisconnectedPlayer(NetworkMessage netMsg)
        {
            // Unregister client from server
            for(int i = 0; i < _connectedClients.Count; i++)
            {
                IdentityTag t = _connectedClients[i];

                if(netMsg.conn.connectionId == t.conn.connectionId)
                {
                    _connectedClients.RemoveAt(i);
                    break;
                }
            }

            Debug.Log("[ValleyNet] Server received Disconnect [src: " + netMsg.conn.address  + "(" + netMsg.conn.connectionId + ")]");
            OnServerDisconnectedPlayer(new ConnectionEventArgs(netMsg.conn));
            //NetworkServer.SendToAll(MessageType.DisconnectNotice, new );
        }
    }
}