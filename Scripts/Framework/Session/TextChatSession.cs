namespace ValleyNet.Framework.Session
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using ValleyNet.Core.Session;
    using ValleyNet.Map;
    using ValleyNet.Framework.Gamemode;
    using ValleyNet.Core.Tag;


    public class TextChatSession : Session
    {
        /* Events */
        public event EventHandler MessageReceived;
        public event EventHandler MessageSent;
        /**********/
        private List<IdentityTag> _members;


        public TextChatSession(string roomName) : base("CHAT", roomName)
        {
            _members = new List<IdentityTag>();
            Debug.Log("[ValleyNet] Text Chat Session " + roomName);
        }

        
        public bool JoinChat(IdentityTag id)
        {
            if(!_members.Contains(id))
            {
                _members.Add(id);
                return true;
            }

            return false;
        }
    }
}