namespace ValleyNet.Core.Server.Event
{
    using System;
    using UnityEngine.Networking;
    using ValleyNet.Core.Session;

    public class ConnectionEventArgs : EventArgs
    {
        public NetworkConnection conn {get; private set;}

        public ConnectionEventArgs(NetworkConnection conn)
        {
            this.conn = conn;
        }
    }



    public class NetworkMessageEventArgs : EventArgs
    {
        public NetworkMessage msg {get; private set;}

        public NetworkMessageEventArgs(NetworkMessage msg)
        {
            this.msg = msg;
        }
    }


    public class SessionEventArgs : EventArgs
    {
        public Session session {get; private set;}

        public SessionEventArgs(Session s)
        {
            this.session = s;
        }
    }
}