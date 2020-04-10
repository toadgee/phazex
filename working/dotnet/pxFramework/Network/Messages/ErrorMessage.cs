using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* error message
	 * 
	 * NAME							BYTES
	 * type							1
	 * length						2
	 * message						1-1024
	 */
    public class ErrorMessage: Message
    {
        public delegate void ErrorMessageDelegate(string msg);

        private string _text;
        public string Text { get { return _text; } }
        public ErrorMessage(byte [] b) : base(b)
        {
            Validate();
            int length = (int)msg[1];
            length <<= 8;
            length += (int)msg[2];
            _text = Message.ByteArrayToString(msg, 3, (short)length);
        }

        public ErrorMessage(string s) : base()
        {
            if ((s.Length < 1) || (s.Length > 1024))
            {
                throw new Exception("Error message is of invalid length.");
            }

            _text = s;

            msg = new byte[3 + s.Length];
            msg[0] = (byte)pxMessages.ErrorMessage;
            msg[1] = (byte)(s.Length >> 8);
            msg[2] = (byte)s.Length;
            Message.StringToByteArray(s, msg, 3);
        }

        public override string ToString()
        {
            return "ERROR : " + _text;
        }

        public void Validate()
        {
            if (msg.Length < 3)
            {
                throw new BadMessageException("Error Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.ErrorMessage)
            {
                throw new BadMessageException("Error Message : Message does not start with the proper type");
            }
            int tmp;
            tmp = (int)msg[1];
            tmp <<= 8;
            tmp += (int)msg[2];
            if ((tmp < 1) || (tmp > 1024))
            {
                throw new BadMessageException("Error Message : Message length field is invalid");
            }

            if (msg.Length != (tmp + 3))
            {
                throw new BadMessageException("Error Message : Message length is invalid");
            }
        }
    }
}
