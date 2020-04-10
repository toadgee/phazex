using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* get game rules message
	 *
	 * NAME                         BYTES
	 * type                         1
	 */
    public class GetGameRulesMessage : Message
    {
        public GetGameRulesMessage() : base()
        {
            msg = new byte[] { (byte)pxMessages.GetGameRules };
        }

        public GetGameRulesMessage(byte [] b) : base(b)
        {
            Validate();
        }

        public override string ToString()
        {
            return "Get Game Rules request";
        }

        public void Validate()
        {
            if (msg.Length != 1)
            {
                throw new BadMessageException("Get Game Rules Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.GetGameRules)
            {
                throw new BadMessageException("Get Game Rules Message : Message does not start with the proper type");
            }
        }
    }
}
