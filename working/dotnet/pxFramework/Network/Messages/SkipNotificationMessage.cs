using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /*  skip notification message
	 * 
	 * NAME							BYTES
	 * type							1
	 * player id					1
	 *
	 */
    public class SkipNotificationMessage : Message
    {
        public delegate void SkipNotificationMessageDelegate(Player p);

        private int _playerId;
        public int PlayerId { get { return _playerId; } }
        public SkipNotificationMessage(byte [] b, int [] ids) : base(b)
        {
            Validate(ids);
            _playerId = (int)msg[1];
        }

        public SkipNotificationMessage(int player_id) : base()
        {
            _playerId = player_id;

            msg = new byte[2];
            msg[0] = (byte)pxMessages.SkipNotification;
            msg[1] = (byte)player_id;

        }

        public override string ToString()
        {
            return "Skip Notification for [" + _playerId + "]";
        }

        public void Validate(int [] ids)
        {
            if (msg.Length != 2)
            {
                throw new BadMessageException("Skip Notification Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.SkipNotification)
            {
                throw new BadMessageException("Skip Notification Message : Message does not start with the proper type");
            }

            int tmp = (int)msg[1];
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
                throw new BadMessageException("Skip Notification Message : Unknown player ID");
            }
        }
    }
}
