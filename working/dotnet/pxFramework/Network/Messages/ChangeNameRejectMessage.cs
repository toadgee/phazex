using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* Change Name reject message
	 * 
	 * NAME							BYTES
	 * type							1
	 *
	 */
    public class ChangeNameRejectMessage : Message
    {
        public delegate void ChangeNameRejectMessageDelegate();

        public ChangeNameRejectMessage(byte [] b) : base(b)
        {
            Validate();
        }

        public ChangeNameRejectMessage() : base()
        {
            msg = new byte[] { (byte)pxMessages.ChangeNameReject };
        }


        public override string ToString()
        {
            return "Rejected change name request";
        }

        public void Validate()
        {
            if (msg.Length != 1)
            {
                throw new BadMessageException("Change Name Reject Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.ChangeNameReject)
            {
                throw new BadMessageException("Change Name Reject Message : Message does not start with the proper type");
            }
        }
    }
}
