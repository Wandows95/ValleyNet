namespace ValleyNet.Core.Server.Component
{
    using UnityEngine;
    using ValleyNet.Core.Server;

    public class ValleyServerComponent : MonoBehaviour
    {
        protected ValleyServer _server;

        [SerializeField]
        private int _serverPort = 8888;
        [SerializeField]
        protected string _serverName = "";
        [SerializeField]
        protected string _serverMOTD = "";
        private ServerConfig _conf;

        void Awake()
        {
            _conf = new ServerConfig();
            _conf.serverPort = _serverPort;
            _conf.serverName = _serverName;
            _conf.serverMOTD = _serverMOTD;
            _server = ValleyServer.Instance;
            Listen();
        }


        public void Listen()
        {
            _server.Listen(_conf, _serverPort);
        }
    }
}