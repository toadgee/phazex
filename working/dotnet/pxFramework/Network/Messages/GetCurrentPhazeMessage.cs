using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* get current phaze message
	 *
	 * NAME							BYTES
	 * type							1
	 */
    public class GetCurrentPhazeMessage : Message
    {
        public GetCurrentPhazeMessage() : base()
        {
            msg = new byte[] { (byte)pxMessages.GetCurrentPhaze };
        }

        public GetCurrentPhazeMessage(byte [] b) : base(b)
        {
            Validate();
        }

        public override string ToString()
        {
            return "Get Current Phaze request";
        }

        public void Validate()
        {
            if (msg.Length != 1)
            {
                throw new BadMessageException("Get Current Phaze Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.GetCurrentPhaze)
            {
                throw new BadMessageException("Get Current Phaze Message : Message does not start with the proper type");
            }
        }
    }
}
