namespace ValleyNet.Entity.Player
{
    using UnityEngine.Networking;
    using ValleyNet.Entity;
    
    public class PlayerTag : EntityTag
    {
        protected string username;


        public PlayerTag(NetworkInstanceId netId, int updatePriority, string username="") : base(netId, updatePriority)
        {
            this.username = username;
        }
    }
}