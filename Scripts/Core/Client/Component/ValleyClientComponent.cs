/*
*   Basic MonoBehaviour hook into ValleyClient
*/
namespace ValleyNet.Core.Client.Component
{
    using UnityEngine;
    using ValleyNet.Core.Client;
    using ValleyNet.Core.Protocol.Message;


    public class ValleyClientComponent : MonoBehaviour
    {
        protected ValleyClient _client; // Reference to ValleyClient

        [SerializeField]
        private string _serverIp = "127.0.0.1";
        [SerializeField]
        private int _serverPort = 8888;
        [SerializeField]
        protected string _username = "VN_USER";
        [Tooltip("Automatically register ValleyClient's built-in LLAPI NetworkHandlers?")]
        public bool useBaseNetworkHandlers = true;

        public bool isConnected{get{return (_client==null) ? false : _client.isConnected;}}


        void Start()
        {
            ProfileMessage profile = new ProfileMessage();  // Build profile for client
            profile.username = _username;
            _client = new ValleyClient(profile, useBaseNetworkHandlers); // Create client for profile
            Connect();
        }


        public void Connect()
        {
            if(!isConnected)
            {
                _client.Connect(_serverIp, _serverPort);
            }
        }
    }
}