namespace ValleyNet.Framework.Session
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using ValleyNet.Core.Session;
    using ValleyNet.Core.Asset;
    using ValleyNet.Map;
    using ValleyNet.Framework.Gamemode.Server;

    public class GamemodeSession : Session
    {
        /* EVENTS */
        public event EventHandler MapChange;
        public event EventHandler GamemodeChange;
        /**********/
        private List<MapTag> _mapPlaylist;        
        private GamemodeScheduler _scheduler;

        public GamemodeSession(string tag, string name, GamemodeScheduler scheduler=null) : base(tag, name)
        {
            _mapPlaylist = new List<MapTag>();

            if(scheduler != null)
            {
                _scheduler = scheduler;
            }
            else
            {
                GameObject gm = new GameObject("Session_"+id+"'s Gamemode Scheduler");
                scheduler = gm.AddComponent<GamemodeScheduler>();
            }

            _mapPlaylist = new List<MapTag>();
        }

        public void RegisterSessionPrefabs(GameObject[] prefabs)
        {
            if(_scheduler.currentPhase == "Loading")
            {
                AssetManager.RegisterPrefabs(prefabs);
            }
        }

        // Sets Gamemode as the next gamemode to be schduled
        public void QueueNextGamemode()
        {
            //_config.nextGamemode = gamemode;
        }

        // Add map to playlist queue
        public void QueueMap(MapTag newMap)
        {
            _mapPlaylist.Add(newMap);
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
    }
}