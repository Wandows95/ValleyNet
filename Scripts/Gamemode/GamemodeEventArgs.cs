namespace ValleyNet.Gamemode
{
    using System;

    public class GamemodeEventArgs : EventArgs
    {
        public string phaseName;
        public bool didTimeout;

        public GamemodeEventArgs(string phaseName, bool didTimeout = false)
        {
            this.phaseName = phaseName;
            this.didTimeout = didTimeout;
        }
    }
}