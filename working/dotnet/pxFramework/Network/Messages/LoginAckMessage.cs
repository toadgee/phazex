using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* login acknowledgement message
	 *
	 * NAME						    BYTES
	 * type                         1
	 * player id                    1
	 * 
	 */
    public class LoginAckMessage: Message
    {
        private int _id;
        public int Id { get { return _id; } }
        public LoginAckMessage(byte [] b) : base(b)
        {
            Validate();
            _id = (int)msg[1];
        }

        public LoginAckMessage(int id)
            : base()
        {
            _id = id;

            msg = new byte[2];
            msg[0] = (byte)pxMessages.LoginAcknowledgment;
            msg[1] = (byte)id;
        }

        public override string ToString()
        {
            return "Login acknowledgement [" + _id + "]";
        }

        public void Validate()
        {
            if (msg.Length != 2)
            {
                throw new BadMessageException("Login Ack Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.LoginAcknowledgment)
            {
                throw new BadMessageException("Login Ack Message : Message does not start with the proper type");
            }
        }
    }
}
