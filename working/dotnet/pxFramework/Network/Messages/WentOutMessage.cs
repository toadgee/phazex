using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /*  went out message
	 * 
	 * NAME							BYTES
	 * type							1
	 * player id					1
	 *
	 */
    public class WentOutMessage : Message
    {
        public delegate void WentOutMessageDelegate(Player p);

        private int _playerId;
        public int PlayerId { get { return _playerId; } }
        public WentOutMessage(byte [] b, int [] ids) : base(b)
        {
            Validate(ids);
            _playerId = (int)msg[1];
        }

        public WentOutMessage(int player_id) : base()
        {
            _playerId = player_id;

            msg = new byte[2];
            msg[0] = (byte)pxMessages.WentOut;
            msg[1] = (byte)player_id;
        }

        public override string ToString()
        {
            return "Went Out Message [" + _playerId + "]";
        }

        public void Validate(int [] ids)
        {
            if (msg.Length != 2)
            {
                throw new BadMessageException("Went Out Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.WentOut)
            {
                throw new BadMessageException("Went Out Message : Message does not start with the proper type");
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
                throw new BadMessageException("Went Out Message : Unknown player ID");
            }
        }
    }
}
