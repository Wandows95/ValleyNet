/*
*   A session is one instance of a networked "universe".
*   Sessions wrap the Gamemode, which defines the ruleset and game logic
*
*   Server --> Session --> Gamemode --> Match
*/
namespace ValleyNet.Core.Session
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using ValleyNet.Map;
    using ValleyNet.Gamemode;

    public class Session
    {   
        private static int _nextId = 0;

        /* Events */
        public event EventHandler SessionStart;
        public event EventHandler MapChange;
        public event EventHandler GamemodeChange;
        public event EventHandler SessionEnd;
        /* Class Variables */
        protected SessionSettings _settings;
        [SerializeField]
        protected GamemodeScheduler scheduler;         // Scheduling Component (Plays the Gamemode)
        private int _id;
        /* Getters & Settings */
        public int id{ get{return _id;} }
        

        public Session(GamemodeScheduler scheduler=null)
        {
            _id = _nextId++;
            _settings = new SessionSettings();
            _settings.mapPlaylist = new List<MapTag>();

            if(scheduler != null)
            {
                this.scheduler = scheduler;
            }
            else
            {
                GameObject gm = new GameObject("Session_"+_id+"'s Gamemode Scheduler");
                scheduler = gm.AddComponent<GamemodeScheduler>();
            }
        }


        // Sets Gamemode as the next gamemode to be schduled
        public void QueueNextGamemode(IGamemode gamemode)
        {
            _settings.nextGamemode = gamemode;
        }


        // Add map to playlist queue
        public void QueueMap(MapTag newMap)
        {
            _settings.mapPlaylist.Add(newMap);
        }


        protected virtual void OnSessionStart(EventArgs eventArgs)
        {
            EventHandler handler = SessionStart;

            if(handler != null)
            {
                handler(this, eventArgs);
            }
        }


        protected virtual void OnMapChange(EventArgs eventArgs)
        {
            EventHandler handler = MapChange;

            if(handler != null)
            {
                handler(this, eventArgs);
            }
        }


        protected virtual void OnGamemodeChange(EventArgs eventArgs)
        {
            EventHandler handler = GamemodeChange;

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


    // Holds the settings data for Session
    public struct SessionSettings
    {
        public List<MapTag> mapPlaylist;            // Map playlist 'queue'
        public IGamemode gamemode, nextGamemode;    // Current & Next Gamemode
    }
}