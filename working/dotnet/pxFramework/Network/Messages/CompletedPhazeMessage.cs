using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /*  melded message
	 * 
	 * NAME							BYTES
	 * type							1
	 * player id					1
	 *
	 */
    public class CompletedPhazeMessage : Message
    {
        public delegate void CompletedPhazeMessageDelegate(Player p);
        private int _playerId;
        public int PlayerId { get { return _playerId; } }
        public CompletedPhazeMessage(byte [] b, int [] ids) : base(b)
        {
            Validate(ids);
            _playerId = (int)msg[1];
        }
        
        public CompletedPhazeMessage(int player_id) : base()
        {
            _playerId = player_id;

            msg = new byte[2];
            msg[0] = (byte)pxMessages.CompletedPhaze;
            msg[1] = (byte)player_id;
        }

        public override string ToString()
        {
            return "Completed Phaze [" + _playerId + "]";
        }

        public void Validate(int [] ids)
        {
            if (msg.Length != 2)
            {
                throw new BadMessageException("Completed Phaze Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.CompletedPhaze)
            {
                throw new BadMessageException("Completed Phaze Message : Message does not start with the proper type");
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
                throw new BadMessageException("Completed Phaze Message : Unknown player ID");
            }
        }
    }
}
