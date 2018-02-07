namespace ValleyNet.Core.Server
{
    using UnityEngine;
    using System.Collections.Generic;


    [CreateAssetMenu(fileName = "ServerSettings", menuName = "ValleyNet/ServerSettings", order = 0)]
    public class ServerSettings : ScriptableObject
    {
        public string serverName = "";
        public string serverMOTD = "";
        [HideInInspector]
        public int serverPort = 8888;
        [HideInInspector]
        public string MOTD = "Powered by ValleyNet";
        public List<string> bannedIP; 
        public List<string> whitelistedIP;
        public List<string> whitelistedUsers;
        public bool runInBackground = true;

        public int tickrate = 64; // Ticks per second
    }
}