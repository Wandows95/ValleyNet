namespace ValleyNet.Framework.Server.Chat
{
    using System;
    using System.Collection.Generic;
    using UnityEngine.Networking;
    using ValleyNet.Core.Server;
    using ValleyNet.Entity.Tag;


    public class ServerChatChannel
    {
        /* EVENTS */
        public event EventHandler UserJoinedChannel;
        public event EventHandler UserLeftChannel;
        /**********/
        protected List<IdentityTag> _members; // Holds connection ids
        protected SortedSet<ChatMessage> _chatBuffer;
        protected ValleyServer _server;
        private string _channelName;

        public HashSet<IdentityTag> members { get{return _members;} }
        public string channelName           { get{return _channelName;} }


        public ServerChatChannel(string channelName)
        {
            _server = ValleyServer.Instance;
            _members = new List<IdentityTag>();
            _chatBuffer = new List<ChatMessage>();
            _channelName = channelName;
        }


        public virtual bool AddMember(IdentityTag newMem)
        {
            return _members.Add(newMem);
        }


        public virtual bool RemoveMember(IdentityTag rmMem)
        {
            return _members.Remove(rmMem);
        }


        public virtual void BroadcastMessage(IdentityTag sender, ChatMessage msg)
        {
            if(_members.Contains(sender))
            {
                SendToChannel(msg);
            }
        }


        // Used for channel log output, private DM, etc
        public virtual void DirectMessage(IdentityTag sender, IdentityTag receiver, ChatMessage msg)
        {
            if(_members.Contains(sender) && _members.Contains(receiver))
            {
                _server.SendToClient(receiver.conn.connectionId, msg, MessageType.ChatMessage);
                BufferMessage(msg);
            }
        }


        protected virtual void SendToChannel(ChatMessage msg)
        {
            NetworkChatMessage msg = new NetworkChatMessage(msg.payload, msg.username, _channelName);

            foreach(IdentityTag t in _members)
            {
                _server.SendToClient(t.conn.connetionId, msg, MessageType.ChatMessage);
            }

            BufferMessage(msg);
        }


        protected virtual void BufferMessage(ChatMessage msg)
        {
            _chatBuffer.Add(msg);
        }
    }
}