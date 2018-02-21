/*  
*   Tracks order and timing of Gamemode phases.
*   If a Gamemode is the Blu-Ray, this is the Blu-Ray player
*       - Event GameModeChangePhase raised when phase changes
*/
namespace ValleyNet.Framework.Gamemode.Server
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using ValleyNet.Core.Session;
    using ValleyNet.Framework.Gamemode;
    using ValleyNet.Framework.Gamemode.Phase;

    public class GamemodeScheduler : MonoBehaviour
    {
        public event EventHandler GamemodeChangePhase; // Gamemode Change Event

        [SerializeField]
        private string _gamemodeName;

        protected Session _session;         // Game session this scheduler is scheduling
        protected Gamemode _gamemode;      // Gamemode this scheduler is enacting 
        protected PhaseQueue _phaseQueue;   // Queue that holds our phase "schedule"

        private int _phaseTimer = 1000;

        protected PhaseData[] _corePhases = 
        {
            new PhaseData("Loading", 0f), // 0f == no timelimit, ended manually
            new PhaseData("Lobby", 0f), 
            new PhaseData("Post-Lobby", 0f), 
            new PhaseData("Play", 0f), 
            new PhaseData("Post-Play", 0f)
        };

        public int phaseTimer {get{return _phaseTimer;}}
        public string currentPhase {get{return _phaseQueue.currentPhase;}}

        void Awake()
        {
            _gamemode = new Gamemode();
            _phaseQueue = new PhaseQueue(_corePhases, _gamemode.phases);
        }
    
        public void ForceNewGamemode(Gamemode nextGamemode, string playPhaseName="Play")
        {
            _gamemode = nextGamemode;
            _gamemodeName = _gamemode.name;
            Debug.Log("[ValleyNet] New Gamemode \'" + nextGamemode.name + "\' Forcibly Scheduled");
            _phaseQueue.Clear(); // Clear phase queue schedule
            _phaseQueue.BuildNewPhaseQueue(_corePhases, _gamemode.phases, playPhaseName); // build new phase queue from new gamemode
        }

        // Trigger phase change event
        protected virtual void OnGamemodeChangePhase(GamemodeEventArgs eventArgs)
        {
            EventHandler handler = GamemodeChangePhase;

            if(handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void NextPhase(bool didTimeout=false)
        {
            if(_phaseTimer > 0)
            {
                StopCoroutine("TimePhase");
            }

            OnGamemodeChangePhase(new GamemodeEventArgs(_phaseQueue.NextPhase(), didTimeout)); // Raise phase change event
            StartCoroutine("TimePhase", _corePhases[_phaseQueue.phaseNum].duration); // Begin countdown of new phase
        }

        // raiseTimeout : Flag if phase ended via timeout (e.g. "Loading" phase timeout to prevent infinite loading screen)
        private IEnumerator TimePhase(float time, bool raiseTimeout)
        {
            if(time==0.0f)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(1);
                
                if(_phaseTimer == 0)
                {
                    NextPhase(raiseTimeout);
                    yield return null;
                }
                else
                {
                    _phaseTimer--;
                }
            }
        }
    }
    
    /*
    *   Phase schedule tracking structure.
    *   Compiles gamemode phase schedule from core and gamemode layer
     */
    [Serializable]
    public struct PhaseQueue
    {
        private List<PhaseData> _phases;
        private int _currentPhase;

        public int phaseNum{ get{return _currentPhase; }}
        public string currentPhase {get{return _phases[_currentPhase].name;}}

        public PhaseQueue(PhaseData[] core, PhaseData[] play, string playPhaseName="Play")
        {
            _currentPhase = 0;
            _phases = new List<PhaseData>();
            BuildNewPhaseQueue(core, play, playPhaseName);
        }

        public void BuildNewPhaseQueue(PhaseData[] core, PhaseData[] gamemodePhases, string playPhaseName="Play")
        {
            int insertIndex = 0;

            _phases.InsertRange(0, core); // populate w/ Core Phases

            Debug.Log("[ValleyNet] Building new Phase Schedule, injecting Gamemode's schedule into " + playPhaseName);

            // Search for Injection Phase
            for(int i = 0; i < _phases.Count; i++)
            {
                if(_phases[i].name == playPhaseName)
                {
                    insertIndex = i+1;
                }
            }

            // Inject into list to build final schedule
            _phases.InsertRange(insertIndex, gamemodePhases);
        }

        // Get next phase, wraps around
        public string NextPhase()
        {
            _currentPhase++;

            if(_currentPhase >= _phases.Count)
            {
                _currentPhase = 0;
            }
            
            return _phases[_currentPhase].name;
        }

        // Inject phase into the Queue @ index
        public bool Insert(int index, PhaseData newPhase)
        {
            if(index < 0 || index >= _phases.Count)
            {
                return false;
            }

            _phases.Insert(index, newPhase);
            return true;
        }

        // Add phase to end of Queue
        public void Append(PhaseData newPhase)
        {
            _phases.Add(newPhase);
        }

        // Flush phase queue
        public void Clear()
        {
            _phases.Clear();
        }
    }
}