using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /*  game over message
	 * 
	 * NAME							BYTES
	 * type							1
	 *
	 */
    public class GameOverMessage : Message
    {
        public delegate void GameOverMessageDelegate();
        public GameOverMessage() : base()
        {
            msg = new byte[] { (byte)pxMessages.GameOver };
        }

        public GameOverMessage(byte [] b) : base(b)
        {
            Validate();
        }

        public override string ToString()
        {
            return "Game Over";
        }

        public void Validate()
        {
            if (msg.Length != 1)
            {
                throw new BadMessageException("Game Over Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.GameOver)
            {
                throw new BadMessageException("Game Over Message : Message does not start with the proper type");
            }
        }
    }
}
