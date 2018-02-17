/*
*   MessageBase inbox/outbox message stream
*       - Allows for message filtering
*/
namespace ValleyNet.Core.Network.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Networking;


    public class Mailbox<T> where T : NetworkMessage
    {
        /* EVENTS */
        public event EventHandler messageReceived;
        /**********/
        protected List<T> _messageBuffer;
        protected IDataFilter<T> _inFilter; // Input modifier (e.g. Decrypt, Decompress)
        protected IDataFilter<T> _outFiter; // Output modifier (e.g. Encrypt, Compress)
        protected IMessageCourier _courier;
        private T _outgoing;
        private int _bufferSize; // in messages
        private int _id;
        private int _msgType;
        private bool _isServer;
        private bool _dirty;

        public int id {get{return _id;}}
        public T outgoing{set{_dirty = true; _outgoing = value;}}

        public Mailbox(int id, short msgType, int bufferSize = 10, IDataFilter<T> inFilter = null, IDataFilter<T> outFilter = null)
        {
            _bufferSize = bufferSize;
            _messageBuffer = new List<T>();
            _outFilter = outFilter;
            _inFilter = inFilter;
            _msgType = msgType;
            _id = id;
            _dirty = false;
        }

        protected virtual void Send()
        {
            if(_dirty)
            {
                _courier.Send(_outgoing.msgType, _outgoing);
                _dirty = false;
            }
        }

        protected virtual void OnMessageReceived()
        {
            EventHandler handler = messageReceived;

            if(messageReceived != null)
            {
                handler(this, new EventArgs());
            }
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
    }

    public interface IDataFilter<T> where T : MessageBase
    {
        T Filter(T in);
    }

    public interface IMessageCourier
    {
        void RegisterHandler(short msgType, NetworkMessageDelegate handler);
        void Send(short msgType, MessageBase msg);
    }
}