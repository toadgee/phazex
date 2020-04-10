using System;
using System.Collections.Generic;
using System.Text;
using PhazeX.Options;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /* Hand message
 	 *
	 * NAME                                                 BYTES
	 * type                                                 1
	 * replacement											1
	 * number of cards                                      1
	 * card color                                           1 *
	 * card number                                          1 *
	 * card iswild                                          1 *
	 * card id                                              2 *
	 */
    public class HandMessage : Message
    {
        public delegate void HandMessageDelegate(bool replacement);
        public HandMessage(Hand h, bool replacement) : base()
        {
            _hand = h;
            _replacement = replacement;

            msg = new byte[3 + (h.Count * 5)];
            msg[0] = (byte)pxMessages.Hand;
            
            if (replacement) msg[1] = (byte)1;
            else msg[1] = (byte)0;
            
            msg[2] = (byte)(h.Count);

            short ptr = 3;
            foreach (Card c in h)
            {
                msg[ptr] = (byte)c.Color;
                msg[ptr + 1] = (byte)c.Number;
                
                if (c.Wild) msg[ptr + 2] = (byte)1;
                else msg[ptr + 2] = (byte)0;

                msg[ptr + 3] = (byte)(c.Id >> 8);
                msg[ptr + 4] = (byte)(c.Id);
                
                ptr += 5;
            }
        }


        private bool _replacement;
        public bool Replacement { get { return _replacement; } }

        private Hand _hand;
        public Hand HandObj { get { return _hand; } }

        public HandMessage(byte [] b, GameRules rules) : base(b)
        {
            Validate(rules);
            _replacement = ((int)msg[1] == 1);
            _hand = new Hand();
            Card c = null;
            int cards = (int)(msg[2]);
            int msgctr = 3;

            int id = 0;
            int col = 0;
            int num = 0;

            for (int ctr = 0; ctr < cards; ctr++)
            {
                id = (int)msg[msgctr + 3];
                id <<= 8;
                id += (int)msg[msgctr + 4];

                num = (int)msg[msgctr + 1];
                col = (int)msg[msgctr];

                if (1 == ((int)(msg[msgctr + 2])))
                {
                    c = new Card(CardNumber.Wild, (CardColor)col, id);
                    c.Number = (CardNumber)num;
                }
                else
                {
                    c = new Card((CardNumber)num, (CardColor)col, id);
                }
                msgctr += 5;
                _hand.Add(c);
            }
        }

        public override string ToString()
        {
            string retval = "";
            int i = 0;
            if (_hand != null)
            {
                foreach (Card c in _hand)
                {
                    i++;
                    if (retval != "") retval += ", ";
                    retval += c.ToString();
                }
            }
            return "Hand (" + i + ") : " + retval;
        }

        public void Validate(GameRules rules)
        {
            if (msg[0] != (byte)pxMessages.Hand)
            {
                throw new BadMessageException("Hand Message : Message does not start with the proper type");
            }
            if (msg.Length < 3)
            {
                throw new BadMessageException("Hand Message : Message length is invalid");
            }

            int tmp = (int)msg[1];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Hand Message : Replacement value is invalid");
            }

            tmp = (int)msg[2];
            if ((tmp < 1) || (tmp > rules.HandCards + 1))
            {
                throw new BadMessageException("Hand Message : Number of cards encoded is invalid (" + tmp + ")");
            }
            if (msg.Length != ((tmp * 5) + 3))
            {
                throw new BadMessageException("Hand Message : Message length is invalid");
            }

            int num_cards = tmp;
            int pos = 3;

            for (int ctr = 0; ctr < num_cards; ctr++)
            {
                tmp = (int)msg[pos]; //card color
                if ((tmp < (int)rules.MinimumCardColor) || (tmp > (int)rules.MaximumCardColor))
                {
                    throw new BadMessageException("Hand Message : Card color field is invalid");
                }

                tmp = (int)msg[pos + 1]; //card number
                if ((tmp < (int)rules.MinimumCardNumber) || (tmp > (int)rules.MaximumCardNumber))
                {
                    throw new BadMessageException("Hand Message : Card number field is invalid");
                }

                tmp = (int)msg[pos + 2]; //wild
                if ((tmp != 0) && (tmp != 1))
                {
                    throw new BadMessageException("Hand Message : Card wild field is invalid");
                }

                tmp = (int)msg[pos + 3];
                tmp <<= 8;
                tmp += (int)msg[pos + 4];
                if ((tmp < 0) || (tmp > rules.DeckCards))
                {
                    throw new BadMessageException("Hand Message : Card ID field is invalid");
                }

                pos += 5;
            }
        }
    }
}
