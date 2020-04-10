using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;
using PhazeX.Options;

namespace PhazeX.Network.Messages
{
    /*  got discard message
	 * 
	 * NAME							BYTES
	 * type							1
	 * player id					1
	 * card color					1
	 * card number					1
	 * card iswild					1
	 * card ID						2
	 *
	 */
    public class GotDiscardMessage : Message
    {
        public delegate void GotDiscardMessageDelegate(Player p, Card c);
        public GotDiscardMessage(int player_id, Card c) : base()
        {
            _playerId = player_id;
            _card = c;

            msg = new byte[7];
            msg[0] = (byte)pxMessages.GotDiscard;
            msg[1] = (byte)player_id;
            msg[2] = (byte)c.Color;
            msg[3] = (byte)c.Number;
            
            if (c.Wild) msg[4] = (byte)1;
            else msg[4] = (byte)0;

            msg[5] = (byte)(c.Id >> 8);
            msg[6] = (byte)(c.Id);
        }

        private int _playerId;
        public int PlayerId { get { return _playerId; } }
        private Card _card;
        public Card CardObj { get { return _card; } }

        public GotDiscardMessage(byte [] b, int [] ids, GameRules rules) : base(b)
        {
            Validate(ids, rules);
            _playerId = (int)msg[1];
            int id = (((int)msg[5]) << 8) + ((int)msg[6]);
            if ((int)msg[4] == 1)
            {
                _card = new Card(CardNumber.Wild, (CardColor)msg[2], id);
                _card.Number = (CardNumber)msg[3];
            }
            else
            {
                _card = new Card((CardNumber)msg[3], (CardColor)msg[2], id);
            }
        }

        public override string ToString()
        {
            return "Got Discard [" + _playerId + "]";
        }

        public void Validate(int [] ids, GameRules rules)
        {
            if (msg.Length != 7)
            {
                throw new BadMessageException("Got Discard Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.GotDiscard)
            {
                throw new BadMessageException("Got Discard Message : Message does not start with the proper type");
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
                throw new BadMessageException("Got Discard Message : Unknown player ID");
            }

            tmp = (int)msg[2];
            if ((tmp < (int)rules.MinimumCardColor) || (tmp > (int)rules.MaximumCardColor))
            {
                throw new BadMessageException("Got Discard Message : Card color field is invalid");
            }

            tmp = (int)msg[3];
            if ((tmp < (int)rules.MinimumCardNumber) || (tmp > (int)rules.MaximumCardNumber))
            {
                throw new BadMessageException("Got Discard Message : Card number field is invalid");
            }

            tmp = (int)msg[4];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Got Discard Message : wild field is invalid");
            }

            tmp = (int)msg[5];
            tmp <<= 8;
            tmp += (int)msg[6];
            if ((tmp < 0) || (tmp >= rules.DeckCards))
            {
                throw new BadMessageException("Got Discard Message : Card ID field is invalid");
            }
        }
    }
}
