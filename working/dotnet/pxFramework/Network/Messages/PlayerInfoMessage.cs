using System;
using System.Collections.Generic;
using System.Text;

namespace PhazeX.Network.Messages
{
    /* player info message
	 *
	 * NAME							BYTES
	 * type							1
	 * user id						1
	 * client release				1
	 * client revision				1
	 * age							1
	 * male / female (1 / 0)        1
	 * email length					1
	 * email						0-255
	 * info length					1
	 * info							0-255
	 */
    public class PlayerInfoMessage : Message
    {
        private int _userid;
        public int UserId { get { return _userid; } }
        private int _release;
        public int Release { get { return _release; } }
        private int _revision;
        public int Revision { get { return _revision; } }
        private int _age;
        public int Age { get { return _age; } }
        private bool _male;
        public bool Male { get { return _male; } }
        private string _email;
        public string Email { get { return _email; } }
        private string _info;
        public string Info { get { return _info; } }

        public PlayerInfoMessage(byte [] b, int [] ids) : base(b)
        {
            Validate(ids);
            _userid = (int)msg[1];
            _release = (int)msg[2];
            _revision = (int)msg[3];
            _age = (int)msg[4];
            _male = (1 == (int)msg[5]);
            _email = Message.ByteArrayToString(msg, 7, (short)msg[6]);
            _info = Message.ByteArrayToString(msg, (short)(9 + (short)msg[6]), (short)(8 + (short)msg[6]));
        }

        public PlayerInfoMessage(int userid, int rel, int rev, int age, bool male, string email, string info)
            : base()
        {
            _userid = userid;
            _release = rel;
            _revision = rev;
            _age = age;
            _male = male;
            _email = email;
            _info = info;

            if (email.Length > 255)
            {
                throw new Exception("Email is invalid length");
            }
            if (info.Length > 255)
            {
                throw new Exception("Info is invalid length");
            }

            msg = new byte[8 + info.Length + email.Length];
            msg[0] = (byte)pxMessages.PlayerInfo;
            msg[1] = (byte)userid;
            msg[2] = (byte)rel;
            msg[3] = (byte)rev;
            msg[4] = (byte)age;

            if (male) msg[5] = (byte)1;
            else msg[5] = (byte)0;

            msg[6] = (byte)email.Length;
            int ptr = Message.StringToByteArray(email, msg, 7);
            msg[ptr] = (byte)info.Length;
            Message.StringToByteArray(email, msg, ptr + 1);
        }

        public override string ToString()
        {
            return "Player Info for " + _userid;
        }

        public void Validate(int [] ids)
        {
            if (msg.Length < 7)
            {
                throw new BadMessageException("Player Info Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.PlayerInfo)
            {
                throw new BadMessageException("Player Info Message : Message does not start with the proper type");
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
                throw new BadMessageException("Player Info Message : Unknown player ID");
            }

            tmp = (int)msg[5];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Player Info Message : Male/Female field is invalid");
            }

            tmp = 6;
            tmp += (int)msg[6];
            tmp += 1;
            tmp += (int)msg[tmp];
            if (tmp != msg.Length)
            {
                throw new BadMessageException("Player Info Message : Message length is invalid");
            }
        }
    }
}
