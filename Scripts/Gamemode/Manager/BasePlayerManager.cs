namespace ValleyNet.Gamemode.Manager
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using ValleyNet.Core.Server;
    using ValleyNet.Core.Entity.Player;


    public class BasePlayerManager : MonoBehaviour
    {
        protected Gamemode _currentMode;
        Dictionary<uint, PlayerTag> _playerList;

        void Awake()
        {
            _playerList = new Dictionary<uint, PlayerTag>();
        }


        void Start()
        {
            //Server.GetInstance().ServerAddedPlayer += ;
        }

        
        protected virtual void OnServerAddedPlayer(EventArgs e)
        {
            Debug.Log("Player Added");
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


        public bool RegisterPlayer(uint netId, PlayerTag p)
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
            else if(_playerList.ContainsKey(netId))
            {
                Debug.Log("[ValleyNet] Player Manager: Unable to add duplicate player [" + p.username + "].");
            }
            else
            {
                _playerList.Add(netId, p);
                return true;
            }

            return false;
        }
    }
}