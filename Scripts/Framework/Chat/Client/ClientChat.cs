namespace ValleyNet.Framework.Client.Chat
{
    using ValleyNet.Core.Client;
    using System.Collections.Generic;

    public class ClientChat : IChatChannelListener
    {
        protected List<ClientChatChannel> _channels;
        protected List<ChatMessage> _messageBuffer;
        protected ValleyClient _client;

        public ClientChat(ValleyClient client, ClientChatChannel[] channels)
        {
            _channels = new List<ClientChatChannel>(channels);
            _messageBuffer = new List<ChatMessage>();
            _client = client;
        }

        public virtual void ReceiveChannelMessage(ChatMessage msg)
        {
            _messageBuffer.Add(msg);
        }
    }


    public interface IChatChannelListener
    {
        void ReceiveChannelMessage(ChatMessage msg);
    }
}