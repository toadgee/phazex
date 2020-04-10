// this file has been ported to the OSX framework
// this file needs updating in the OSX framework

namespace PhazeX
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines the state of a game.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// The game has been created but not started.
        /// </summary>
        GameInitialized,

        /// <summary>
        /// The hand is starting.
        /// </summary>
        HandStarting,

        PlayerPickingCard,
        
        PlayerPlaying,

        PlayerDiscarding,

        TurnStarting,

        TurnEnding,

        HandEnding,
        
        GameEnding,

        /// <summary>
        /// The game is over.
        /// </summary>
        EndOfGame
    }
}
