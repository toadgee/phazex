using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /* got deck card message
	 * 
	 * NAME							BYTES
	 * type							1
	 * player id					1
	 *
	 */
    public class GotDeckCardMessage : Message
    {
        public delegate void GotDeckCardMessageDelegate(Player p);

        private int _playerId;
        public int PlayerId { get { return _playerId; } }
        public GotDeckCardMessage(byte [] b, int [] ids) : base(b)
        {
            Validate(ids);
            _playerId = (int)msg[1];
        }

        public GotDeckCardMessage(int player_id) : base()
        {
            _playerId = player_id;

            msg = new byte[2];
            msg[0] = (byte)pxMessages.GotDeckCard;
            msg[1] = (byte)player_id;

        }

        public override string ToString()
        {
            return "Got Deck Card [" + _playerId + "]";
        }

        public void Validate(int [] ids)
        {
            if (msg.Length != 2)
            {
                throw new BadMessageException("Got Deck Card Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.GotDeckCard)
            {
                throw new BadMessageException("Got Deck Card Message : Message does not start with the proper type");
            }
        }
    }
}
