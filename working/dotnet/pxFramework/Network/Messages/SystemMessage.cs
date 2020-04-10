using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* system message
	 * 
	 * NAME							BYTES
	 * type							1
	 * length						2
	 * message						1-1024
	 */
    public class SystemMessage : Message
    {
        public delegate void SystemMessageDelegate(string msg);
        private string _text;
        public string Text { get { return _text; } }
        public SystemMessage(byte [] b) : base(b)
        {
            Validate();
            int length = (int)msg[1];
            length <<= 8;
            length += (int)msg[2];
            _text = Message.ByteArrayToString(msg, 3, (short)length);
        }

        public SystemMessage(string s) : base()
        {
            _text = s;

            if ((s.Length < 1) || (s.Length > 1024))
            {
                throw new Exception("Message is invalid");
            }
            msg = new byte[3 + s.Length];
            msg[0] = (byte)pxMessages.SystemMessage;
            msg[1] = (byte)(s.Length >> 8);
            msg[2] = (byte)(s.Length);
            Message.StringToByteArray(s, msg, 3);
        }

        public override string ToString()
        {
            return "<" + _text + ">";
        }

        public void Validate()
        {
            if (msg.Length < 3)
            {
                throw new BadMessageException("System Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.SystemMessage)
            {
                throw new BadMessageException("System Message : Message does not start with the proper type");
            }
            int tmp;
            tmp = (int)msg[1];
            tmp <<= 8;
            tmp += (int)msg[2];
            if ((tmp < 1) || (tmp > 1024))
            {
                throw new BadMessageException("System Message : Message length field is invalid");
            }

            if (msg.Length != (tmp + 3))
            {
                throw new BadMessageException("System Message : Message length is invalid");
            }
        }
    }
}
