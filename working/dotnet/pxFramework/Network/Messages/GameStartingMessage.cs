using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* game starting message
	 *
	 * NAME							BYTES
	 * type							1
	 */
    public class GameStartingMessage : Message
    {
        public delegate void GameStartingMessageDelegate();
        public GameStartingMessage() : base()
        {
            msg = new byte[] { (byte)pxMessages.GameStarting };
        }

        public GameStartingMessage(byte [] b) : base(b)
        {
            Validate();
        }

        public override string ToString()
        {
            return "Game Starting message";
        }

        public void Validate()
        {
            if (msg.Length != 1)
            {
                throw new BadMessageException("Game Starting Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.GameStarting)
            {
                throw new BadMessageException("Game Starting Message : Message does not start with the proper type");
            }
        }
    }
}
