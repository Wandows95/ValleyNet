namespace ValleyNet.Core.Server.Event
{
    using System;
    using UnityEngine.Networking;

    public class ConnectionEventArgs : EventArgs
    {
        public NetworkConnection conn {get; private set;}

        public ConnectionEventArgs(NetworkConnection conn)
        {
            this.conn = conn;
        }
    }
}