namespace ValleyNet.Core.Entity.Player
{
    using UnityEngine.Networking;
    using ValleyNet.Core.Entity;
    
    public class PlayerTag : EntityTag
    {
        protected string _username;
        public string username {get{return _username;}}

        public PlayerTag(NetworkInstanceId netId, int updatePriority, string username="") : base(netId, updatePriority)
        {
            this._username = username;
        }
    }
}