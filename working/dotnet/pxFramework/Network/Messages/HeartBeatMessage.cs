using System;
using System.Collections.Generic;
using System.Text;
using PhazeX.Network;

namespace PhazeX.Network.Messages
{
    /* heartbeat message
	 *
	 * NAME                                                 BYTES
	 * type                                                 1
	 */
    public class HeartBeatMessage : Message
    {
        public delegate void HeartbeatMessageDelegate();
        public HeartBeatMessage() : base()
        {
            msg = new byte[] { (byte)pxMessages.Heartbeat };
            _log = false;
        }

        public HeartBeatMessage(byte [] b) : base(b)
        {
            Validate();
            _log = false;
            //nothing to expand
        }

        public override string ToString()
        {
            return "Heartbeat";
        }

        private void Validate()
        {
            if (msg.Length != 1)
            {
                throw new BadMessageException("Heartbeat Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.Heartbeat)
            {
                throw new BadMessageException("Heartbeat Message : Message does not start with the proper type");
            }
        }
    }
}
