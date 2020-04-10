using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /* turn end message
     * 
     * NAME							BYTES
     * type							1
     * user id					    1
     */
    public class TurnEndMessage : Message
    {
        public delegate void TurnEndMessageDelegate(Player p);

        private int _id;
        public int Id { get { return _id; } }
        public TurnEndMessage(byte [] b, int [] ids) : base(b)
        {
            Validate(ids);
            _id = (int)msg[1];
        }

        public TurnEndMessage(int userid) : base()
        {
            _id = userid;

            msg = new byte[2];
            msg[0] = (byte)pxMessages.TurnEnd;
            msg[1] = (byte)userid;
        }

        public override string ToString()
        {
            return "Turn End [" + _id + "]";
        }

        public void Validate(int [] ids)
        {
            if (msg.Length != 2)
            {
                throw new BadMessageException("Turn End Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.TurnEnd)
            {
                throw new BadMessageException("Turn End Message : Message does not start with the proper type");
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
                throw new BadMessageException("Turn End Message : Unknown player ID");
            }
        }
    }
}
