namespace ValleyNet.Entity.Sync
{
    public class NetSyncSampler <T> : INetSync<T>
    {
        T target;

        public NetSyncSampler(T target)
        {
            this.target = target;
        }

        
        public T SampleInput() // Get input sampled
        {
            return target;
        }         
        
        
        public T DirectSampleInput()   // Force re-sample of input and get it
        {
            return target;
        }
    }
}