/*
*   The Server acts as the outermost layer in our game server ( Server(Session(Gamemode(Match))) )
*
*   Responsibilities:
*       1. Invoke handler logic for New Connections
*       2. Enforce the network level rules (max connections, banned ips, whitelists, send rate)
 */
namespace ValleyNet.Server
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;
    using ValleyNet.Protocol.Message;
    using ValleyNet.Entity.Player;
    using ValleyNet.Session;

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


    public class Server : MonoBehaviour
    {
        private Server _instance;
        private NetworkServer _server;
        
        /* EVENTS */
        public event EventHandler ServerAddedMainSession;   // Most games will only need the main session
        public event EventHandler ServerAddedNewSession;    // Raised if server adds a NON-MAIN session (multiple game sessions on one server)
        public event EventHandler ServerRemovedSession;     // Raised if server removes a NON-MAIN session (a main session exists until end of server)
        public event EventHandler ServerConnectedPlayer;
        public event EventHandler ServerAddedPlayer;
        public event EventHandler ServerDisconnectedPlayer;
        /**********/
        private short _serverState;                         // Current state in the server lifecycle
        [SerializeField]
        private ServerSettings _settings;
        List<Session> _activeSessions;
        List<Session> _pausedSessions;

        public short serverState { get{return _serverState;} }
        public Session mainSession { get{return _activeSessions[0];} }
        

        public static Server GetInstance()
        {
            if(_instance == null)
            {
                GameObject go = Instantiate("ValleyNet Server");
                _instance = go.AddComponent<Server>();
            }
            
            return _instance;
        }


        void Awake()
        {
            _serverState = ServerState.Setup;
            _activeSessions = new List<Session>();
            _pausedSessions = new List<Session>();

            _activeSessions.Add(new Session());
            OnServerAddedMainSession(new EventArgs());

            Time.fixedDeltaTime = (float)1/_settings.tickrate;
            Debug.Log("[ValleyNet] Server Tickrate: " + _settings.tickrate + " (" + Time.fixedDeltaTime + "ms)" );

            StartServer();
        }


        public bool StartServer()
        {
            if(!NetworkServer.active)
            {
                BindBaseNetworkHandlers();
                Application.runInBackground = _settings.runInBackground;
                NetworkServer.Listen(_settings.serverPort);
                Debug.Log("[ValleyNet] Server listening on " + _settings.serverPort);
                return true;
            }

            return false;
        }


        // Register Handlers for basic server functionality
        private void BindBaseNetworkHandlers()
        {
            NetworkServer.RegisterHandler(MsgType.Connect, NetRaiseServerConnectedPlayer); // Player Connects
            NetworkServer.RegisterHandler(MsgType.AddPlayer, NetRaiseServerAddedPlayer); // Player Object is Added
            NetworkServer.RegisterHandler(MessageType.Profile, NetRaiseServerProfileSync); // Player profile is Sync'd
            NetworkServer.RegisterHandler(MsgType.Disconnect, NetRaiseServerDisconnectedPlayer); // Player disconnects
        }


        protected virtual void OnServerConnectedPlayer(EventArgs eventArgs)
        {
            EventHandler handler = ServerConnectedPlayer;

            if(handler != null)
            {
                handler(this, eventArgs);
            }
        }


        protected virtual void OnServerAddedPlayer(EventArgs eventArgs)
        {
            EventHandler handler = ServerAddedPlayer;

            if(handler != null)
            {
                handler(this, eventArgs);
            }
        }


        protected virtual void OnServerDisconnectedPlayer(EventArgs eventArgs)
        {
            EventHandler handler = ServerDisconnectedPlayer;

            if(handler != null)
            {
                handler(this, eventArgs);
            }
        }


        protected virtual void OnServerAddedMainSession(EventArgs eventArgs)
        {
            EventHandler handler = ServerAddedMainSession;

            if(handler != null)
            {
                handler(this, eventArgs);
            }
        }


        protected virtual void OnServerAddedNewSession(EventArgs eventArgs)
        {
            EventHandler handler = ServerAddedNewSession;

            if(handler != null)
            {
                handler(this, eventArgs);
            }
        }


        // Network Facing Wrapper for "ServerConnectedPlayer" Event
        private void NetRaiseServerConnectedPlayer(NetworkMessage netMsg)
        {
            Debug.Log("Received Connection [src: " + netMsg.conn.address  + "(" + netMsg.conn.connectionId + ")]");
            OnServerConnectedPlayer(new EventArgs());
        }


        // Network Facing Wrapper for "ServerAddedPlayer" Event
        private void NetRaiseServerAddedPlayer(NetworkMessage netMsg)
        {
            OnServerAddedPlayer(new EventArgs());
        }


        // Network Facing Wrapper for "ServerDisconnectedPlayer" Event
        private void NetRaiseServerDisconnectedPlayer(NetworkMessage netMsg)
        {
            OnServerDisconnectedPlayer(new EventArgs());
        }


        private void NetRaiseServerProfileSync(NetworkMessage netMsg)
        {
            ProfileMessage profileMessage = netMsg.ReadMessage<ProfileMessage>();
            // TO-DO Invoke ServerProfileSync, spawn playertag, register new player
        }
    }
}