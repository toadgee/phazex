using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;
using PhazeX.Network;

namespace PhazeX.Network.Messages
{
    /* ready message
	 *
	 * NAME							BYTES
	 * type							1
	 * userid						1
	 * ready?						1 (0 - no, 1 - yes)
	 */
    public class ReadyMessage : Message
    {
        public delegate void ReadyMessageDelegate(Player p);

        private int _id;
        public int Id { get { return _id; } }
        private bool _ready;
        public bool Ready { get { return _ready; } }

        public ReadyMessage(byte [] b, int [] ids) : base(b)
        {
            Validate(ids);
            _id = (int)msg[1];
            _ready = ((int)msg[2] == 1);
        }

        public ReadyMessage(int userid, bool ready) : base()
        {
            _id = userid;
            _ready = ready;

            msg = new byte[3];
            msg[0] = (byte)pxMessages.Ready;
            msg[1] = (byte)userid;
            if (ready) msg[2] = (byte)1;
            else msg[2] = (byte)0;
        }

        public override string ToString()
        {
            string retval = "Player [" + _id + "] is ";
            if (_ready) retval += "ready";
            else retval += "not ready";
            return retval;
        }

        public void Validate(int [] ids)
        {
            if (msg.Length != 3)
            {
                throw new BadMessageException("Ready Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.Ready)
            {
                throw new BadMessageException("Ready Message : Message does not start with the proper type");
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
                throw new BadMessageException("Ready Message : Unknown player ID");
            }

            tmp = (int)msg[2];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Ready Message : Ready field is invalid!");
            }
        }
    }
}
