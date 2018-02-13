namespace ValleyNet.Gamemode.Manager
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using ValleyNet.Core.Server;
    using ValleyNet.Core.Server.Event;
    using ValleyNet.Core.Tag;
    using ValleyNet.Framework.Gamemode;


    public class BasePlayerManager
    {
        /* EVENTS */
        public event EventHandler MinimumPlayersReached; // Raised when minimum # of players are registered
        public event EventHandler MaximumPlayersReached; // Raised when maximum # of players are registered
        /**********/
        protected Gamemode _currentMode;
        List<IdentityTag> _playerList;
        private ValleyServer _server;

        private int _maxPlayers;
        private int _minPlayers;

        public BasePlayerManager(int minPlayers, int maxPlayers)
        {
            _playerList = new List<IdentityTag>();

            _server = ValleyServer.Instance;
            _server.ServerRequestedAddPlayer += OnServerRequestedAddPlayer;
        }
        

        protected virtual void OnServerRequestedAddPlayer(object sender, NetworkMessageEventArgs e)
        {
            Debug.Log("Player Attempting Add");
        }


        public bool RegisterGamemode(Gamemode m)
        {
            if(_currentMode == null)
            {
                _currentMode = m;
                return true;
            }

            return false;
        }


        public virtual bool RegisterPlayer(IdentityTag p)
        {
            if(_currentMode == null)
            {
                Debug.Log("[ValleyNet] Player Manager: Unable to register player with no registered gamemode");
            }
            else if(_currentMode.maxPlayers >= _playerList.Count)
            {
                //TO-DO
                Debug.Log("TODO");
            }
            else if(_playerList.Contains(p))
            {
                Debug.Log("[ValleyNet] Player Manager: Unable to add duplicate player [" + p.username + "].");
            }
            else
            {
                _playerList.Add(p);
                return true;
            }

            return false;
        }
    }
}