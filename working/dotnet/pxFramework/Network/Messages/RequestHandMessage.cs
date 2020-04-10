using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* request hand message
	 * 
	 * NAME                         BYTES
	 * type                         1
	 */
    public class RequestHandMessage : Message
    {
        public RequestHandMessage(byte [] b) : base(b)
        {
            Validate();
        }

        public RequestHandMessage() : base()
        {
            msg = new byte[] { (byte)pxMessages.RequestHand };
        }

        public override string ToString()
        {
            return "Request hand";
        }

        public void Validate()
        {
            if (msg.Length != 1)
            {
                throw new BadMessageException("RequestHand Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.RequestHand)
            {
                throw new BadMessageException("RequestHand Message : Message does not start with the proper type");
            }
        }
    }
}
