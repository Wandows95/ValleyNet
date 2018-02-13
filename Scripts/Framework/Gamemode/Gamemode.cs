namespace ValleyNet.Framework.Gamemode
{
    using System;
    using ValleyNet.Framework.Gamemode.Phase;
    

    public class Gamemode 
    {
        // Phases cannot be named "Loading", "Lobby", "Post-Lobby","Play or "Post-Play"
        // List of play phases, in order of occurrance, that happen in one game or round
        protected PhaseData[] _phases = { new PhaseData("Main", 0f) };
        protected float _playDuration = 600.0f; // If mode has no phases, this determines timelimit in seconds. Otherwise, adds up phase durations
        protected string _name="Gamemode Name";
        protected string _description="Gamemode";
        protected int _minPlayers = 2;
        protected int _maxPlayers = 8;

        public string name              {get{return _name;}}
        public string description       {get{return _description;}}
        public int minPlayers           {get{return _minPlayers;}}
        public int maxPlayers           {get{return _maxPlayers;}}
        public bool hasPhases           {get{if(_phases != null) {return (_phases.Length > 0);} return false;}}
        public PhaseData[] phases       {get{if(_phases!= null){return _phases;} else return new PhaseData[1];}}
        

        public Gamemode(GamemodeParams overrideParams)
        {
            LoadParams(overrideParams);
            _playDuration = GetPlayDuration();
        }


        public Gamemode()
        {
            _playDuration = GetPlayDuration();
        }


        public float GetPlayDuration()
        {
            float duration = 0;

            if(hasPhases)
            {
                for(int i = 0; i < _phases.Length; i++)
                {
                    duration += _phases[i].duration;
                }
            }
            else
            {
                duration = _playDuration;
            }

            return duration;
        }


        private void LoadParams(GamemodeParams _params)
        {
            if(_params.maxPlayers > 0 && _params.maxPlayers >= _params.minPlayers)
            {
                _maxPlayers = _params.maxPlayers;
                _minPlayers = _params.minPlayers;
            }
        }


        // EVENT(src: GamemodeScheduler) Raised when scheduler has changed phase
        protected virtual void OnGamemodeChangePhase(GamemodeEventArgs eventArgs)
        {
            // Get eventArgs.phaseName and implement logic based on which phase has been passed to you
        }
    }


    /*
    *   Struct that contains override parameters for gamemode.
    *   Used to easily mutatate the gamemode at runtime (e.g. change min/max players, DM vs TDM, etc.)
     */
    public struct GamemodeParams
    {
        public int minPlayers;
        public int maxPlayers;
        public string iconDirectory; // Location of mode's display icon

        public GamemodeParams(int minPlayers=0, int maxPlayers=0, string iconDirectory="")
        {
            this.minPlayers = minPlayers;
            this.maxPlayers = maxPlayers;
            this.iconDirectory = iconDirectory;
        }
    }
}

namespace ValleyNet.Framework.Gamemode.Phase
{
    public struct PhaseData
    {
        public string name;
        public float duration;

        public PhaseData(string name, float duration)
        {
            this.name = name;
            this.duration = duration;
        }
    }
}