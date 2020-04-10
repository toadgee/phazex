using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* get top deck card message
	 *
	 * NAME							BYTES
	 * type							1
 	 */
    public class GetTopDeckCardMessage :Message
    {
        public GetTopDeckCardMessage() : base()
        {
            msg = new byte[] { (byte)pxMessages.GetTopDeckCard };
        }

        public GetTopDeckCardMessage(byte [] b) : base(b)
        {
            Validate();
        }

        public override string ToString()
        {
            return "Get Top Deck Card";
        }

        public void Validate()
        {
            if (msg.Length != 1)
            {
                throw new BadMessageException("Get Top Deck Card Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.GetTopDeckCard)
            {
                throw new BadMessageException("Get Top Deck Card Message : Message does not start with the proper type");
            }
        }
    }
}
