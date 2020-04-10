using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* dialog message
	 * 
	 * NAME							BYTES
	 * type							1
	 * length						2
	 * message						1-1024
	 */
    public class DialogMessage : Message
    {
        public delegate void DialogMessageDelegate(string msg);

        private string _text;
        public string Text { get { return _text; } }
        public DialogMessage(byte [] b) : base(b)
        {
            Validate();
            _text = Message.ByteArrayToString(msg, 2, (short)msg[1]);
        }
        public DialogMessage(string s) : base()
        {
            if ((s.Length < 1) || (s.Length > 1024))
            {
                throw new Exception("Dialog message is not valid length");
            }

            _text = s;
            msg = new byte[3 + s.Length];
            msg[0] = (byte)pxMessages.DialogMessage;
            msg[1] = (byte)(s.Length >> 8);
            msg[2] = (byte)(s.Length);
            Message.StringToByteArray(s, msg, 3);
        }

        public override string ToString()
        {
            return "Dialog Message : " + _text;
        }

        public void Validate()
        {
            if (msg.Length < 3)
            {
                throw new BadMessageException("Dialog Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.DialogMessage)
            {
                throw new BadMessageException("Dialog Message : Message does not start with the proper type");
            }
            int tmp;
            tmp = (int)msg[1];
            tmp <<= 8;
            tmp += (int)msg[2];
            if ((tmp < 1) || (tmp > 1024))
            {
                throw new BadMessageException("Dialog Message : Message length field is invalid ");
            }

            if (msg.Length != (tmp + 3))
            {
                throw new BadMessageException("Dialog Message : Message length encoded is invalid");
            }
        }
    }
}
