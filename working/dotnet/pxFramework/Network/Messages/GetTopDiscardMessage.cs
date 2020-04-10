using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* get top discard message
	 * 
	 * NAME							BYTES
	 * type							1
	 */
    public class GetTopDiscardMessage : Message
    {
        public GetTopDiscardMessage() : base()
        {
            msg = new byte[] { (byte)pxMessages.GetTopDiscard };
        }

        public GetTopDiscardMessage(byte [] b) : base(b)
        {
            Validate();
        }

        public override string ToString()
        {
            return "Get Top Discard";
        }

        public void Validate()
        {
            if (msg.Length != 1)
            {
                throw new BadMessageException("Get Top Discard Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.GetTopDiscard)
            {
                throw new BadMessageException("Get Top Discard Message : Message does not start with the proper type");
            }
        }
    }
}
