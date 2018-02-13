namespace ValleyNet.Framework.Session
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using ValleyNet.Core.Tag;
    using ValleyNet.Core.Session;
    using ValleyNet.Map;
    using ValleyNet.Framework.Gamemode;
    

    public class TextVoteSession : Session
    {
        private int _voteTimer, _maxVotes;
        private string _voteText, _voteDescription;
        private string[] _choices;
        private Dictionary<IdentityTag, int> _votes;


        public TextVoteSession(string voteName, string voteText, string voteDescription, string[] choices, int maxVotes, int voteTimeSeconds) : base(voteName, "VOTE")
        {
            _voteTimer = voteTimeSeconds;
            _voteText = voteText;
            _voteDescription = voteDescription;
            _choices = choices;
            maxVotes = _maxVotes;

            _votes = new Dictionary<IdentityTag, int>();

            Debug.Log("[ValleyNet] Text Vote Session \'" + voteName + "\' opened (Topic: " + voteText + ")");
        }


        public bool CastVote(IdentityTag idTag, int choice)
        {
            if(choice > -1 && choice < _choices.Length)
            {
                _votes[idTag] = choice;

                if(_votes.Count == _maxVotes)
                {
                    EndVote(); // flag vote ended
                }

                return true;
            }

            return false;
        }

        
        private string DetermineWinner()
        {
            int[] castedVotes = new int[_choices.Length];
            int highestIndex = 0;

            foreach(KeyValuePair<IdentityTag, int> vote in _votes)
            {
                castedVotes[vote.Value]++;

                if(castedVotes[vote.Value] > highestIndex)
                {
                    highestIndex = vote.Value;
                }
            }

            return _choices[highestIndex];
        }


        protected void EndVote()
        {
            OnSessionEnd(new TextVoteResultArgs(DetermineWinner()));
        }
    }


    public class TextVoteResultArgs : EventArgs
    {
        public string result{get; private set;}


        public TextVoteResultArgs(string result)
        {
            this.result = result;
        }
    }
}