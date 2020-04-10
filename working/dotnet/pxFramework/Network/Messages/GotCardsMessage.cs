using System;
using System.Collections.Generic;
using System.Text;
using PhazeX.Options;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /* got cards message
	 *
	 * NAME                                                 BYTES
	 * type                                                 1
	 * number of cards                                      1
	 * card color                                           1 *
	 * card number                                          1 *
	 * card iswild                                          1 *
	 * card id                                              2 *
	 */
    public class GotCardsMessage : Message
    {
        public delegate void GotCardsMessageDelegate(CardCollection cards);

        private CardCollection _cards;
        public CardCollection Cards { get { return _cards; } }

        public GotCardsMessage(byte [] b, GameRules rules) : base(b)
        {
            Validate(rules);
            Card c = null;
            int cards = (int)(msg[1]);
            _cards = new CardCollection(cards);
            int msgctr = 2;

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
                _cards.Add(c);
                msgctr += 5;
            }
        }

        public GotCardsMessage(CardCollection cc)
            : base()
        {
            _cards = cc;

            short length = (short)(2 + (cc.Count * 5));

            msg = new byte[length];
            msg[0] = (byte)pxMessages.GotCards;
            msg[1] = (byte)cc.Count;


            short ptr = 2;
            foreach (Card c in cc)
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

        public override string ToString()
        {
            string retval = "";
            foreach (Card c in _cards)
            {
                if (retval != "") retval += ", ";
                retval += c.ToString();
            }
            return "Got cards (" + _cards.Count + ") : " + retval;
        }

        public void Validate(GameRules rules)
        {
            if (msg.Length < 2)
            {
                throw new BadMessageException("Got Cards Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.GotCards)
            {
                throw new BadMessageException("Got Cards Message : Message does not start with the proper type");
            }
            

            int tmp = (int)msg[1];
            if ((tmp < 1) || (tmp > rules.HandCards))
            {
                throw new BadMessageException("Got Cards Message : Number of cards encoded is invalid");
            }
            if (msg.Length != ((tmp * 5) + 2))
            {
                throw new BadMessageException("Got Cards Message : Message length is invalid");
            }

            int num_cards = tmp;
            int pos = 2;

            for (int ctr = 0; ctr < num_cards; ctr++)
            {
                tmp = (int)msg[pos]; //card color
                if ((tmp < (int)rules.MinimumCardColor) || (tmp > (int)rules.MaximumCardColor))
                {
                    throw new BadMessageException("Got Cards Message : Card color field is invalid");
                }

                tmp = (int)msg[pos + 1]; //card number
                if ((tmp < (int)rules.MinimumCardNumber) || (tmp > (int)rules.MaximumCardNumber))
                {
                    throw new BadMessageException("Got Cards Message : Card number field is invalid");
                }

                tmp = (int)msg[pos + 2]; //wild
                if ((tmp != 0) && (tmp != 1))
                {
                    throw new BadMessageException("Got Cards Message : Card wild field is invalid");
                }

                tmp = (int)msg[pos + 3];
                tmp <<= 8;
                tmp += (int)msg[pos + 4];
                if ((tmp < 0) || (tmp > rules.DeckCards))
                {
                    throw new BadMessageException("Got Cards Message : Card ID field is invalid");
                }

                pos += 5;
            }

        }
    }
}
