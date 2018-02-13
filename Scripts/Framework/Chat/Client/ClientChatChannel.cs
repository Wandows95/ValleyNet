namespace ValleyNet.Framework.Client.Chat
{
    using System;
    using System.Collection.Generic;
    using UnityEngine.Networking;
    using ValleyNet.Core.Client;

    // SIZE: 64 or 128 bit (8 or 16 bytes)
    public struct ChatMessage
    {
        public string payload   {get; protected set;} // 32 or 64 bit
        public string username  {get; protected set;} // 32 or 64 bit

        public ChatMessage(string payload, string username)
        {
            this.payload = payload;
            this.username = username;
        }
    }


    public class NetworkChatMessage : MessageBase
    {
        public string payload       {get; protected set;}
        public string username      {get; protected set;}
        public string channelName   {get; protected set;}


        public NetworkChatMessage(string payload, string username, string channelName)
        {
            this.channelName = channelName;
            this.payload = payload;
            this.username = username;
        }
    }


    public class ChatMessageEventArgs : EventArgs
    {
        public string channelName {get; set;}
        public string username {get; set;}
        public string payload {get; set;}
    }


    /*
    *   Represents the client portion of a public, insecure chat channel
    *       - Recieves messages, sends messages and cache's activity
    */
    public class ClientChatChannel
    {
        /* EVENTS */
        public event EventHandler<ChatMessageEventArgs> ReceivedMessage;
        /**********/
        private List<ChatMessage> _chatBuffer;
        private List<IChatChannelListener> _listeners;
        private int _chatBufferSize; // In messages. Data size: ~8/16 bytes * bufferSize
        private string _channelName;
        protected ValleyClient _localClient;

        public List<ChatMessage> chatBuffer {get{return _chatBuffer;}}


        public ClientChatChannel(ValleyClient boundClient, string channelName, uint bufferSize)
        {
            _channelName = channelName;
            _chatBuffer = new List<ChatMessage>();
            _chatBufferSize = (int)bufferSize;
            _localClient = boundClient;
            _listeners = new List<IChatChannelListener>();
        }


        public void RegisterListener(IChatChannelListener listener)
        {
            if(!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }


        public void SendMessage(string payload)
        {
            BuildSendMessage(new ChatMessage(payload, _localClient.username));
        }


        public void ReceiveMessage(ChatMessage msg)
        {
            BufferMessage(msg);
            NotifyListeners(msg);
        }

        
        protected virtual void BuildSendMessage(ChatMessage msg)
        {
            msg.user = _localClient.username;
            _localClient.client.Send(MessageType.ChatMessage, new NetworkChatMessage(msg.payload, msg.username));
            ReceiveMessage(msg); // Receive message sent locally to be displayed
        }


        protected virtual void BufferMessage(ChatMessage msg)
        {
            if(_chatBuffer.Count >= _chatBufferSize)
            {
                _chatBuffer.RemoveAt(0); // Dequeue oldest message
            }

            _chatBuffer.Add(msg);
        }


        private void NotifyListeners(ChatMessage msg)
        {
            foreach(IChatChannelListener listener in _listeners)
            {
                listener.ReceiveChannelMessage(msg);
            }
        }
    }
}