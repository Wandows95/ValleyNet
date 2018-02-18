/*
*   MessageBase inbox/outbox message stream
*       - Allows for message filtering
*/
namespace ValleyNet.Core.Network.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Networking;

    public class Mailbox<T> where T : MessageBase
    {
        /* EVENTS */
        public event EventHandler messageReceived;
        /**********/
        protected List<T> _messageBuffer;
        protected IMessageSerializer<T, NetworkMessage> _inFilter; // Input modifier (e.g. Decrypt, Decompress)
        protected IMessageSerializer<T, T> _outFilter; // Output modifier (e.g. Encrypt, Compress)
        protected IMessageCourier _courier;
        private T _outgoing;
        private int _bufferSize; // in messages
        private int _id;
        private short _msgType;
        private bool _isServer;
        private bool _dirty;

        public int id {get{return _id;}}
        public T outgoing{set{if(value != null){_dirty = true; _outgoing = value;}}}

        public Mailbox(int id, short msgType, IMessageCourier courier, IMessageSerializer<T, NetworkMessage> inFilter, IMessageSerializer<T, T> outFilter = null, int bufferSize = 10)
        {
            _courier = courier;
            _bufferSize = bufferSize;
            _messageBuffer = new List<T>();
            _outFilter = outFilter;
            _inFilter = inFilter;
            _msgType = msgType;
            _id = id;
            _dirty = false;

            _courier.RegisterHandler(msgType, OnMessageReceived);
        }

        public virtual void Send()
        {
            if(_dirty)
            {
                _courier.Send(_msgType, _outgoing);
                _dirty = false;
            }
        }

        protected virtual void OnMessageReceived(NetworkMessage msg)
        {
            T newMsg = _inFilter.Serialize(msg);
            BufferNewMessage(newMsg);

            EventHandler handler = messageReceived;
            if(messageReceived != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected void BufferNewMessage(T msg)
        {
            if(_messageBuffer.Count >= _bufferSize)
            {
                while(_messageBuffer.Count > _bufferSize-1) // Trim and make way for new msg
                {
                    _messageBuffer.RemoveAt(0);
                }
            }

            _messageBuffer.Add(msg);
        }

        public string GetChannelName()
        {
            return "MAILBOX-" + _id;
        }

        public T GetNewestMessage()
        {
            return _messageBuffer[_messageBuffer.Count-1];
        }

        public T GetOldestMessage()
        {
            return _messageBuffer[0];
        }

        public override string ToString()
        {
            return "Mailbox-" + _id + " [Type: " + typeof(T).FullName + ", Messages: " + _messageBuffer.Count + ", Dirty Bit: " + _dirty + "]";
        }
    }

    public interface IMessageSerializer<T, K>
    {
        T Serialize(K input);
    }

    public interface IMessageCourier
    {
        void RegisterHandler(short msgType, NetworkMessageDelegate handler);
        void Send(short msgType, MessageBase msg);
    }
}