using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /* logoff message
	 * 
	 * NAME							BYTES
	 * type							1
	 * userid						1
	 * should remove from list		1 (0, 1)
	 */
    public class LogOffMessage : Message
    {
        public delegate void LogOffMessageDelegate(Player p);
        public LogOffMessage(int id, bool remove) : base()
        {
            _id = id;
            _remove = remove;

            msg = new byte[3];
            msg[0] = (byte)pxMessages.LogOff;
			msg[1] = (byte)id;
			if (remove) msg[2] = (byte)1;
			else        msg[3] = (byte)0;
        }

        private bool _remove;
        public bool Remove { get { return _remove; } }
        private int _id;
        public int Id { get { return _id; } }

        public LogOffMessage(byte [] b, int [] ids) : base(b)
        {
            Validate(ids);
            _id = (int)msg[1];
            _remove = ((int)msg[2] == 1);
        }

        public override string ToString()
        {
            return "Log off message for [" + _id + "]";
        }

        public void Validate(int [] ids)
        {
			if (msg.Length != 3)
			{
				throw new BadMessageException("Logoff Message : Message length is invalid");
			}
            if (msg[0] != (byte)pxMessages.LogOff)
			{
				throw new BadMessageException("Logoff Message : Message does not start with the proper type");
			}
			
			int tmp;

			tmp = (int)msg[2];
			if ((tmp != 0) && (tmp != 1))
			{
				throw new BadMessageException("Logoff Message : Should remove data is invalid");
			}


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
				throw new BadMessageException("Logoff Message : Unknown player ID");
			}
		}
    }
}
