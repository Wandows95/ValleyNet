/*
*   LLAPI NetworkServer Wrapper for ValleyNet
*   
*   This object essentially acts as a base host configuration object in addition
*   to generating C# events based on network events
*/
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


    public class ValleyServer
    {
        private volatile static ValleyServer _instance;
        private volatile static NetworkServer _server;

        public static ValleyServer Instance {get{return _instance;}}
        
        /* EVENTS */
        public event EventHandler                           ServerStartedListening;
        public event EventHandler<SessionEventArgs>         ServerAddedMainSession;   // Most games will only need the main session
        public event EventHandler<SessionEventArgs>         ServerAddedNewSession;    // Raised if server adds a NON-MAIN session (multiple game sessions on one server)
        public event EventHandler<SessionEventArgs>         ServerRemovedSession;     // Raised if server removes a NON-MAIN session (a main session exists until end of server)
        public event EventHandler<ConnectionEventArgs>      ServerConnectedPlayer;
        public event EventHandler<NetworkMessageEventArgs>  ServerReceivedProfile;    // Raised when server receives a client's identity profile
        public event EventHandler<NetworkMessageEventArgs>  ServerAddedPlayer;
        public event EventHandler<ConnectionEventArgs>      ServerDisconnectedPlayer;
        /**********/
        protected short _serverState;              // Current state in the server lifecycle
        protected Dictionary<string, List<Session>> _activeSessions; // Key: Tag, Value: List of sessions
        protected ServerConfig _config;
        protected HostTopology _topology; // UNET Network Topology to use

        public short serverState {get{return _serverState;}}
        public ServerConfig config {get{return _config;}}


        public ValleyServer(ServerConfig config, HostTopology topology=null)
        {
            _topology = topology;
            _config = config;
            _serverState = ServerState.Setup;
            _activeSessions = new Dictionary<string, List<Session>>();
            BindBaseNetworkHandlers();

            if(_instance == null)
            {
                _instance = this;
            }
        }


        // Invokes NetworkServer.Listen and shifts the serverState to LOBBY
        public void Listen(int serverPort)
        {
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

            // If session tag has not been registered
            if(!_activeSessions.TryGetValue(session.tag, out val))
            {
                _activeSessions.Add(session.tag, new List<Session>()); // Register new tag
            }

            val.Add(session); // Add session to activeSessions
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


        // Register Handlers for basic server functionality
        private void BindBaseNetworkHandlers()
        {
            NetworkServer.RegisterHandler(MsgType.Connect, NetRaiseServerConnectedPlayer);       // Player Connects
            NetworkServer.RegisterHandler(MessageType.Identity, NetRaiseServerIdentitySync);       // Player identity is Sync'd
            //NetworkServer.RegisterHandler(MsgType.AddPlayer, NetRaiseServerAddedPlayer);         // Player Object is Added
            NetworkServer.RegisterHandler(MsgType.Disconnect, NetRaiseServerDisconnectedPlayer); // Player disconnects
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


        // Raises 'ServerDisconnectedPlayer' Event
        protected virtual void OnServerDisconnectedPlayer(ConnectionEventArgs eventArgs)
        {
            EventHandler<ConnectionEventArgs> handler = ServerDisconnectedPlayer;

            if(handler != null)
            {
                handler(this, eventArgs);
            }
        }


        // Network Facing , Raises 'ServerConnectedPlayer' Event
        private void NetRaiseServerConnectedPlayer(NetworkMessage netMsg)
        {
            Debug.Log("[ValleyNet] Server received new Connection [src: " + netMsg.conn.address  + "(" + netMsg.conn.connectionId + ")]");
            
            OnServerConnectedPlayer(new ConnectionEventArgs(netMsg.conn));
        }


        // Network Facing, Raises 'ServerDisconnectedPlayer' Event
        private void NetRaiseServerDisconnectedPlayer(NetworkMessage netMsg)
        {
            Debug.Log("[ValleyNet] Server received Disconnect [src: " + netMsg.conn.address  + "(" + netMsg.conn.connectionId + ")]");

            OnServerDisconnectedPlayer(new ConnectionEventArgs(netMsg.conn));
        }


        // Network Facing, Raises 'ServerIdentitySync' Event
        private void NetRaiseServerIdentitySync(NetworkMessage netMsg)
        {
            IdentityMessage identityMessage = netMsg.ReadMessage<IdentityMessage>();
            Debug.Log("[ValleyNet] Server received new profile [src: " + netMsg.conn.address  + "(" + netMsg.conn.connectionId + ")] Username: " + identityMessage.username);
            // TO-DO Invoke ServerProfileSync, spawn playertag, register new player
        }
    }
}