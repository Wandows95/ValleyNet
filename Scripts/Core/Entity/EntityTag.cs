namespace ValleyNet.Core.Entity
{
    using UnityEngine.Networking;

    public class EntityTag
    {
        protected NetworkInstanceId _netId;
        protected int _updatePriority; // 0 is highest priority

        public int netId {get{ return (int)_netId.Value;}}

        public EntityTag(NetworkInstanceId netId, int updatePriority)
        {
            this._netId = netId;
            this._updatePriority = updatePriority;
        }
    }
}