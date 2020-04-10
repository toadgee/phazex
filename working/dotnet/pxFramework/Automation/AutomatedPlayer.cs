namespace PhazeX.Automation
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using PhazeX.Helpers;
    using PhazeX.Network;
    using PhazeX.Options;

    public class AutomatedPlayer : Player
    {
        private Game game;
        private Random rand;
        
        public Player PlayerToSkip
        {
            get;
            set;
        }


        private void GameHandStart(object sender, HandStartEventArgs e)
        {
            this.Tracker = new CardTracker();
        }

        private void GamePutDiscard(object sender, PutDiscardEventArgs e)
        {
            this.Tracker.AddNotWantedCard(e.Player, e.Card);
        }

        private static void Wait()
        {
            int i = TimingDefinitions.pxcpArtificalWait;
            if (i > 0)
            {
                Thread.Sleep(i);
            }
        }
    }
}
