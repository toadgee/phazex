using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;
using PhazeX.Options;

namespace PhazeX.Network.Messages
{
    /* discard message
	 * 
	 * NAME							BYTES
	 * type							1
	 * valid card                   1     0 if null, 1 if not
	 * card color					1
	 * card number					1
	 * card iswild					1
	 * card ID						2
	 */
    public class DiscardMessage : Message
    {

        private Card _card;
        public Card CardObj { get { return _card; } }
        public DiscardMessage(byte [] b, GameRules rules) : base(b)
        {
            Validate(rules);
            if (0 == (int)msg[1])
            {
                _card = null;
            }
            else
            {
                if (1 == ((int)msg[4]))
                {
                    _card = new Card(CardNumber.Wild, (CardColor)msg[2], (int)(msg[6] << 8) + (int)(msg[5]));
                    _card.Number = (CardNumber)msg[3];
                }
                else
                {
                    _card = new Card((CardNumber)msg[3], (CardColor)msg[2], (int)(msg[6] << 8) + (int)(msg[5]));
                }
            }
        }
        public DiscardMessage(Card c) : base()
        {
            _card = c;
            msg = new byte[7];
            msg[0] = (byte)pxMessages.Discard;

            if (c == null)
            {
                // create an empty packet -- just make sure we decode it correctly on the other end!
                msg[1] = 0;
                msg[2] = 0;
                msg[3] = 0;
                msg[4] = 0;
                msg[5] = 0;
                msg[6] = 0;

            }
            else
            {
                msg[1] = 1;
                msg[2] = (byte)c.Color;
                msg[3] = (byte)c.Number;
                if (c.Wild) msg[4] = 1;
                else msg[4] = 0;
                msg[5] = (byte)c.Id;
                msg[6] = (byte)(c.Id >> 8);
            }
        }

        public override string ToString()
        {
            if (_card == null) return "Discard : empty";
            return "Discard : " + _card.ToString();
        }

        public void Validate(GameRules rules)
        {
            if (msg.Length != 7)
            {
                throw new BadMessageException("Discard Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.Discard)
            {
                throw new BadMessageException("Discard Message : Message does not start with the proper type");
            }
            int tmp = (int)msg[1];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Discard Message : Validity field is invalid");
            }
            tmp = (int)msg[2];
            if ((tmp < (int)rules.MinimumCardColor) || (tmp > (int)rules.MaximumCardColor))
            {
                throw new BadMessageException("Discard Message : Card color field is invalid");
            }
            tmp = (int)msg[3];
            if ((tmp < (int)rules.MinimumCardNumber) || (tmp > (int)rules.MaximumCardNumber))
            {
                throw new BadMessageException("Discard Message : Card number field is invalid");
            }
            tmp = (int)msg[4];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Discard Message : Wild field is invalid");
            }
            tmp = (int)msg[5];
            tmp <<= 8;
            tmp += (int)msg[6];
            if ((tmp < 0) && (tmp >= rules.DeckCards))
            {
                throw new BadMessageException("Discard Message : Card ID field is invalid");
            }
        }
    }
}
