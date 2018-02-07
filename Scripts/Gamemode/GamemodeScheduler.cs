/*  
*   Tracks order and timing of Gamemode phases.
*   If a Gamemode is the Blu-Ray, this is the Blu-Ray player
*       - Event GameModeChangePhase raised when phase changes
*/
namespace ValleyNet.Gamemode
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using ValleyNet.Core.Session;
    using ValleyNet.Gamemode;
    using ValleyNet.Gamemode.Phase;


    public class GamemodeScheduler : MonoBehaviour
    {
        public event EventHandler GamemodeChangePhase; // Gamemode Change Event

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

        

        void Start()
        {
            _gamemode = new Gamemode();
            _phaseQueue = new PhaseQueue(_corePhases, _gamemode.phases);
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
        IEnumerator TimePhase(float time, bool raiseTimeout)
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


        public PhaseQueue(PhaseData[] core, PhaseData[] play, string playPhaseName="Play")
        {
            _currentPhase = 0;
            _phases = new List<PhaseData>();
            BuildPhaseQueue(core, play, playPhaseName);
        }


        private void BuildPhaseQueue(PhaseData[] core, PhaseData[] gamemode, string playPhaseName="Play")
        {
            int insertIndex = 0;

            _phases.InsertRange(0, core); // populate w/ Core Phases

            // Search for Injection Phase
            for(int i = 0; i < _phases.Count; i++)
            {
                if(_phases[i].name == playPhaseName)
                {
                    insertIndex = i+1;
                }
            }

            // Inject into list to build final schedule
            _phases.InsertRange(insertIndex, gamemode);
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