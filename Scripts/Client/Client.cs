namespace ValleyNet.Client
{
    using System;
    using UnityEngine;
    using UnityEngine.Networking;
    using ValleyNet.Protocol.Message;

    public class Client : MonoBehaviour
    {
        protected NetworkClient _client;   // UNET's NetworkClient object utilized
        public static Client mainInstance; // Get main ValleyNet Client instance (first made on machine by default)

        /*EVENTS*/
        public event EventHandler ClientRegisteredHandlers;
        public event EventHandler ClientConnected;
        public event EventHandler ClientStartSync;
        public event EventHandler ClientFinishSync;
        public event EventHandler ClientAddedPlayer;
        public event EventHandler ClientDisconnected;
        /********/
        [SerializeField]
        protected string _serverIp = "127.0.0.1";
        [SerializeField]
        protected int _serverPort = 8888;
        [SerializeField]
        protected string _username = "VN_User";
        private int _tickRate = 64;
        private bool _isConnected = false;

        public string serverIp      {get{return _serverIp;}}
        public int serverPort       {get{return _serverPort;}}
        public int tickRate         {get{return _tickRate;}}
        public bool isConnected     {get{return _isConnected;}}

        public NetworkClient uNetClient {get{return _client;}}


        void Awake()
        {
            if(mainInstance == null)
            {
                mainInstance = this;
            }
        }

        void Start()
        {
            Debug.Log("[ValleyNet] Client-Test: Test firing");
            Connect("", -1, "VN_WANDOWS");
        }


        public void Connect(string serverIp="", int serverPort=-1, string username="")
        {
            if(!_isConnected)
            {
                if(_client == null)
                {
                    _client = new NetworkClient();
                }

                _serverIp = (serverIp != "") ? serverIp : this._serverIp;
                _username = (username != "") ? username : this._username;
                _serverPort = (serverPort > -1) ? serverPort : this._serverPort;
                _client.Connect(_serverIp, _serverPort);

                Debug.Log("[ValleyNet] Client(IpBind: " + _serverIp + "): Attempting to connect to " + _serverIp + ":" + _serverPort + " as [" + _username + "]");            
            }
        }


        public bool Send(short msgType, MessageBase msg)
        {
            if(_isConnected && _client != null)
            {
                return _client.Send(msgType, msg);
            }
            
            return false;
        }


        protected virtual void OnClientRegisteredHandlers()
        {
            _client.RegisterHandler(MsgType.Connect, OnClientConnected);

            EventHandler handler = ClientRegisteredHandlers;
            if(handler != null)
            {
                handler(this, new EventArgs());
            }
        }


        protected virtual void OnClientConnected(NetworkMessage netMsg)
        {
            ProfileMessage profileSync = new ProfileMessage();
            profileSync.username = _username;
            
            _client.Send(MessageType.Profile, profileSync);

            Debug.Log("[ValleyNet] Client(IpBind -" + _serverIp + "): Authenticating profile with server " + profileSync);

            EventHandler handler = ClientConnected;
            if(handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }
}