using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* request table message
	 * 
	 * NAME                         BYTES
	 * type                         1
	 */
    public class RequestTableMessage : Message
    {
        public RequestTableMessage(byte [] b) : base(b)
        {
            Validate();
        }

        public RequestTableMessage() : base()
        {
            msg = new byte[] { (byte)pxMessages.RequestTable };
        }

        public override string ToString()
        {
            return "Table request";
        }

        public void Validate()
        {
            if (msg.Length != 1)
            {
                throw new BadMessageException("RequestTable Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.RequestTable)
            {
                throw new BadMessageException("RequestTable Message : Message does not start with the proper type");
            }
        }
    }
}
