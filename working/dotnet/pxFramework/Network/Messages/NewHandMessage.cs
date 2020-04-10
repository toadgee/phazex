using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* new hand message
	 * 
	 * NAME							BYTES
	 * type							1
	 */
    public class NewHandMessage : Message
    {
        public delegate void NewHandMessageDelegate();
        public NewHandMessage() : base()
        {
            msg = new byte[] { (byte)pxMessages.NewHand };
        }

        public NewHandMessage(byte [] b) : base(b)
        {
            Validate();
        }

        public override string ToString()
        {
            return "New Hand notification";
        }

        public void Validate()
        {
            if (msg.Length != 1)
            {
                throw new BadMessageException("New Hand Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.NewHand)
            {
                throw new BadMessageException("New Hand Message : Message does not start with the proper type");
            }
        }
    }
}
