namespace ValleyNet.Entity.Sync
{
    public interface INetSync<T>
    {
        T SampleInput();         // Get input sampled
        T DirectSampleInput();   // Force re-sample of input and get it
    }
}