using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;
using PhazeX.Options;

namespace PhazeX.Network.Messages
{
    /* discard skip message
	 * 
	 * NAME							BYTES
	 * type							1
	 * card color					1
	 * card ID						2
	 * player ID					1
	 */
    public class DiscardSkipMessage : Message
    {
        public delegate void DiscardSkipMessageDelegate(Player playerToBeSkipped);

        private Card _card;
        public Card CardObj { get { return _card; } }
        private int _playerId;
        public int PlayerId { get { return _playerId; } }

        public DiscardSkipMessage(byte [] b, int [] ids, GameRules rules) : base(b)
        {
            Validate(ids, rules);
            int id = (int)msg[2];
            id <<= 8;
            id += (int)msg[3];
            CardColor col = (CardColor)msg[1];
            _card = new Card(CardNumber.Skip, col, id);
            _playerId = (int)msg[4]; 
        }

        public DiscardSkipMessage(Card c, int player_id)
        {
            _card = c;
            _playerId = player_id;
            msg = new byte[5];
            msg[0] = (byte)pxMessages.DiscardSkip;
            msg[1] = (byte)c.Color;
            msg[2] = (byte)(c.Id >> 8);
            msg[3] = (byte)(c.Id);
            msg[4] = (byte)player_id;
        }


        public override string ToString()
        {
            return "Discard skip : player [" + PlayerId + "]";
        }

        public void Validate(int [] ids, GameRules rules)
        {
            if (msg.Length != 5)
            {
                throw new BadMessageException("Discard Skip Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.DiscardSkip)
            {
                throw new BadMessageException("Discard Skip Message : Message does not start with the proper type");
            }
            int tmp;

            tmp = (int)msg[1];
            if ((tmp < (int)rules.MinimumCardColor) || (tmp > (int)rules.MaximumCardColor))
            {
                throw new BadMessageException("Discard Skip Message : Card color field is invalid");
            }

            tmp = (int)msg[2];
            tmp <<= 8;
            tmp += (int)msg[3];
            if ((tmp < 0) || (tmp > rules.DeckCards))
            {
                throw new BadMessageException("Discard Skip Message : Card ID field is invalid");
            }

            tmp = (int)msg[4];
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
                throw new BadMessageException("Discard Skip Message : Unknown player ID");
            }
        }
    }
}
