using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /* chat message
		 *
		 * NAME                                                 BYTES
		 * type                                                 1
		 * user id                                              1
		 * message length                                       2
		 * message                                              1 - 1024
		 */
    public class ChatMessage : Message
    {
        public delegate void ChatMessageDelegate(Player p, string message);

        private int _id;
        public int Id { get { return _id; } }
        private string _text;
        public string Text { get { return _text; } }


        public ChatMessage (byte [] b, int [] ids) : base(b)
        {
            Validate(ids);
            _id = (int)msg[1];
            int length = (int)msg[2];
            length <<= 8;
            length += (int)msg[3];
            _text = Message.ByteArrayToString(msg, 4, (short)length);
        }

        public ChatMessage (int userid, string message) : base()
        {
            if (message.Length < 1)
            {
                throw new Exception("Message length is invalid");
            }
            if (message.Length > 1024)
            {
                throw new Exception("Message length is invalid");
            }

            _id = userid;
            _text = message;

            msg = new byte[4 + message.Length];
            msg[0] = (byte)pxMessages.Chat;
            msg[1] = (byte)(userid);
            msg[2] = (byte)(message.Length >> 8);
            msg[3] = (byte)(message.Length);
            Message.StringToByteArray(message, msg, 4);
        }

        public override string ToString()
        {
            return "[" + _id + "] : " + _text;
        }

        private void Validate(int [] ids)
        {
            if (msg[0] != (byte)pxMessages.Chat)
            {
                throw new BadMessageException("Chat Message : Message does not start with the proper type");
            }
            if ((msg.Length < 4) || (msg.Length > 1028))
            {
                throw new BadMessageException("Chat Message : Message length is invalid");
            }

            int tmp;
            tmp = (int)msg[1];
            bool found = false;
            foreach (int id in ids)
            {
                if (tmp == id)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                throw new BadMessageException("Chat Message : Unknown player ID");
            }


            tmp = (int)msg[2];
            tmp <<= 8;
            tmp += (int)msg[3];
            if ((tmp < 1) || (tmp > 1024))
            {
                throw new BadMessageException("Chat Message encoded message length is invalid");
            }
            if ((tmp + 4) != msg.Length)
            {
                throw new BadMessageException("Chat Message length is invalid");
            }
        }
    }
}
