using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /* turn start message
	 * 
	 * NAME							BYTES
	 * type							1
	 * user id                      1
	 */
    public class TurnStartMessage : Message
    {
        public delegate void TurnStartMessageDelegate(Player p);

        private int _id;
        public int Id { get { return _id; } }

        public TurnStartMessage(byte [] b, int [] ids) : base(b)
        {
            Validate(ids);
            _id = (int)msg[1];
        }

        public TurnStartMessage(int userid) : base()
        {
            _id = userid;
            
            msg = new byte[2];
            msg[0] = (byte)pxMessages.TurnStart;
            msg[1] = (byte)userid;
        }


        public override string ToString()
        {
            return "Turn Start [" + _id + "]";
        }

        public void Validate(int [] ids)
        {
			if (msg.Length != 2)
			{
				throw new BadMessageException("Turn Start Message : Message length is invalid");
			}
            if (msg[0] != (byte)pxMessages.TurnStart)
			{
				throw new BadMessageException("Turn Start Message : Message does not start with the proper type");
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
				throw new BadMessageException("Turn Start Message : Unknown player ID");
			}
        }
    }
}
