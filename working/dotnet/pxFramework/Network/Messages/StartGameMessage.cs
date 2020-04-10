using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* start game message
	 *
	 * NAME                         BYTES
	 * type                         1
	 */
    public class StartGameMessage:Message
    {
        public StartGameMessage(byte [] b) : base(b)
        {
            Validate();
        }

        public StartGameMessage() : base()
        {
            msg = new byte[] { (byte)pxMessages.StartGame };
        }

        public override string ToString()
        {
            return "Start Game Message";
        }

        public void Validate()
        {
            if (msg.Length != 1)
            {
                throw new BadMessageException("Start Game Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.StartGame)
            {
                throw new BadMessageException("Start Game Message : Message does not start with the proper type");
            }
        }
    }
}
