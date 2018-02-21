namespace ValleyNet.Framework.Gamemode
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using ValleyNet.Core.Tag;

    public class PlayerManager
    {
        /* EVENTS */
        public event EventHandler ReachedCapacity;
        /**********/
        private List<PerformanceTag>[] _teams;
        private int _playersPerTeam;
        private int _numTeams;

        public PlayerManager(int numTeams=1, int playersPerTeam=5)
        {
            _teams = new List<PerformanceTag>[numTeams];
            _playersPerTeam = playersPerTeam;
            _numTeams = numTeams;

            for(int i = 0; i < _teams.Length; i++)
            {
                _teams[i] = new List<PerformanceTag>();
            }
        }

        public virtual void AddPlayer(IdentityTag idTag, int targetTeam=1)
        {
            if(targetTeam < _teams.Length && targetTeam > 0 && _teams[targetTeam].Count < _playersPerTeam)
            {
                _teams[targetTeam].Add(new PerformanceTag(idTag));
                CheckReachedCapacity();
            }
        }

        public virtual void AddPlayer(PerformanceTag t, int targetTeam=1)
        {
            if(targetTeam < _teams.Length && targetTeam > 0 && _teams[targetTeam].Count < _playersPerTeam)
            {
                _teams[targetTeam].Add(t);
                CheckReachedCapacity();
            }
        }

        protected virtual void OnReachedCapacity()
        {
            EventHandler handler = ReachedCapacity;
            if(handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected bool CheckReachedCapacity()
        {
            for(int i = 0; i < _teams.Length; i++)
            {
                if(_teams[i].Count < _playersPerTeam)
                {
                    return false;
                }
            }

            OnReachedCapacity();
            return true;
        }
    }

    public class PerformanceTag
    {
        private IdentityTag _idTag;
        public string name {get{return _idTag.username;}}

        public PerformanceTag(IdentityTag idTag)
        {
            _idTag = idTag;
        }
    }
}