using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* goodbye message
	 *
	 * NAME                         BYTES
	 * type                         1
	 */
    public class GoodbyeMessage : Message
    {
        public delegate void GoodbyeMessageDelegate();

        public GoodbyeMessage() : base()
        {
            msg = new byte[] { (byte)pxMessages.Goodbye };
        }

        public GoodbyeMessage(byte [] b) : base(b)
        {
            
        }

        public override string ToString()
        {
            return "Goodbye Message";
        }

        private void Validate()
        {
            if (msg.Length != 1)
            {
                throw new BadMessageException("Goodbye Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.Goodbye)
            {
                throw new BadMessageException("Goodbye Message : Message does not start with the proper type");
            }
        }
    }
}
