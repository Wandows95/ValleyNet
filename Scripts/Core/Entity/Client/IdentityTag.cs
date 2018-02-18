///<file>
///<summary>
///Tagging object used to track an identified network client
///</summary>
///</file>
namespace ValleyNet.Core.Tag
{
    using UnityEngine.Networking;
    
    public class IdentityTag
    {
        protected NetworkConnection _conn;
        protected string _username;
        readonly Permissions _permissionLevel;

        public string username {get{return _username;}}
        public NetworkConnection conn{get{return _conn;}}


        public IdentityTag(NetworkConnection conn, Permissions permissionLevel=Permissions.SPECTATOR, string username="")
        {
            this._username = username;
            this._conn = conn;
            this._permissionLevel = permissionLevel;
        }
    }
    
    public struct IdentityTagData
    {
        public NetworkConnection conn;
        public string username;
        public Permissions permissionLevel;

    }
    

    // Permission level on the server
    public enum Permissions : byte
    {
        SPECTATOR           =0, // Regular Spectator
        SPECTATOR_CASTER    =1, // Priority Spectator
        PLAYER              =2, // Standard Player
        PRO_PLAYER          =3, // Priority Player
        CHAT_MODERATOR      =4,
        GAME_MODERATOR      =5,
        IT_MODERATOR        =6, // IT/Server Network Moderator
        MODERATOR           =7, // General Moderator
        GAME_ADMIN          =8, // Uppermost player manager
        IT_ADMIN            =9, // Server/IT Admin
        OP_ADMIN            =10,// Operations Admin (Server staff, developers)
        ADMIN               =11,// General Admin
        OWNER               =12
    }
}