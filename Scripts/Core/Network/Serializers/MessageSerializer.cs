namespace ValleyNet.Core.Network.Data
{
    public class MessageSerializer<T, K> : IMessageSerializer<T, K>
    {
        public virtual T Serialize(K input)
        {
            return default(T);
            // SERIALIZE HERE
        }
    }
}