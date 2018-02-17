namespace ValleyNet.Framework.Server.Chat
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Networking;
    using ValleyNet.Core.Server;
    using ValleyNet.Core.Tag;
    using ValleyNet.Core.Protocol.Message;
    using ValleyNet.Framework.Client.Chat;

    public class ServerChatChannel
    {
        /* EVENTS */
        public event EventHandler UserJoinedChannel;
        public event EventHandler UserLeftChannel;
        /**********/
        protected List<IdentityTag> _members; // Holds connection ids
        protected HashSet<ChatMessage> _chatBuffer;
        protected ValleyServer _server;
        private string _channelName;

        public List<IdentityTag> members    { get{return _members;} }
        public string channelName           { get{return _channelName;} }


        public ServerChatChannel(string channelName)
        {
            _server = ValleyServer.Instance;
            _members = new List<IdentityTag>();
            _chatBuffer = new HashSet<ChatMessage>();
            _channelName = channelName;
        }

        public virtual void AddMember(IdentityTag newMem)
        {
            _members.Add(newMem);
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
        /*
        public virtual void DirectMessage(IdentityTag sender, IdentityTag receiver, ChatMessage msg)
        {
            NetworkChatMessage chatMsg = new NetworkChatMessage(msg.payload, msg.username, _channelName);

            if(_members.Contains(sender) && _members.Contains(receiver))
            {
                NetworkServer.SendToClient(receiver.conn.connectionId, MessageType.ChatMessage, msg);
                BufferMessage(msg);
            }
        }
        */

        protected virtual void SendToChannel(ChatMessage msg)
        {
            NetworkChatMessage chatMsg = new NetworkChatMessage(msg.payload, msg.username, _channelName);

            foreach(IdentityTag t in _members)
            {
                NetworkServer.SendToClient(t.conn.connectionId, MessageType.ChatMessage, chatMsg);
            }

            BufferMessage(msg);
        }

        protected virtual void BufferMessage(ChatMessage msg)
        {
            _chatBuffer.Add(msg);
        }
    }
}