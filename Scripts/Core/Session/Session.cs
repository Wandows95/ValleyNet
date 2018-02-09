/*
*   A session is one instance of a networked "universe".
*   Sessions wrap the execution instance
*
*   Server --> Session
*/
namespace ValleyNet.Core.Session
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;


    public class Session
    {   
        private static int _nextId = 0;

        /* Events */
        public event EventHandler SessionStart;
        public event EventHandler SessionEnd;
        /**********/
        private int _id;
        private string _tag;    // Custom tag for differentiating session information when casted to <Session> type
                                // Usage: "_tag = "Gamemode" for Gamemode session
        protected string _name; // User string 
        /* Getters & Settings */
        public int id{ get{return _id;} }
        public string name{ get{return _name;} }
        public string tag{ get{return _tag;} }
        

        public Session(string name, string tag)
        {
            _name = name;
            _tag = tag;
            _id = _nextId++;
        }


        protected virtual void OnSessionStart(EventArgs eventArgs)
        {
            EventHandler handler = SessionStart;

            if(handler != null)
            {
                handler(this, eventArgs);
            }
        }


        protected virtual void OnSessionEnd(EventArgs eventArgs)
        {
            EventHandler handler = SessionEnd;

            if(handler != null)
            {
                handler(this, eventArgs);
            }
        }
        
    }
}