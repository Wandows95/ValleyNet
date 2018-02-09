/*
*   Tagging object used to track an identified network client
*/
namespace ValleyNet.Core.Tag
{
    using UnityEngine.Networking;
    
    public class IdentityTag : EntityTag
    {
        protected string _username;
        public string username {get{return _username;}}

        public IdentityTag(NetworkInstanceId netId, int updatePriority, string username="") : base(netId, updatePriority)
        {
            this._username = username;
        }
    }
}