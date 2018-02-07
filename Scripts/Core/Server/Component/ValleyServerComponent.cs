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

        void Awake()
        {
            ServerConfig conf = new ServerConfig();
            conf.serverPort = _serverPort;
            conf.serverName = _serverName;
            conf.serverMOTD = _serverMOTD;
            _server = new ValleyServer(conf);
            _server.Listen(_serverPort);
        }


        public void Listen()
        {
            _server.Listen(_serverPort);
        }
    }
}