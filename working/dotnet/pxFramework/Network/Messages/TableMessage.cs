using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;
using PhazeX.Helpers;
using PhazeX.Options;

namespace PhazeX.Network.Messages
{
    /* table message
     *
     * NAME							BYTES
     * type							1
     * number of groups				1
     * group ID						1 *
     * group kind					1 *
     * group cards required			1 *
     * group cards					1 *
     * card ID						2 **
     * card color					1 **
     * card number					1 **
     * card iswild                  1 **
     */
    public class TableMessage : Message
    {
        public delegate void TableMessageDelegate();

        private Table _table;
        public Table TableObj { get { return _table; } }
        public TableMessage(byte [] b, GameRules rules) : base(b)
        {
            Validate(rules);
            int gmax = (int)msg[1];

            Group g = null;
            Card cd = null;
            _table = new Table();

            int cards = 0;
            int msgctr = 2;
            for (int gctr = 0; gctr < gmax; gctr++)
            {
                int id = (int)msg[msgctr];
                int tmp = (int)msg[msgctr + 1];
                int cr = (int)msg[msgctr + 2];
                cards = msg[msgctr + 3];

                g = Group.Create((GroupType)tmp, cr);
                g.Id = id;

                msgctr = msgctr + 4;

                for (int cctr = 0; cctr < cards; cctr++)
                {
                    id = (int)msg[msgctr];
                    id <<= 8;
                    id += (int)msg[msgctr + 1];

                    int col = (int)msg[msgctr + 2];
                    int num = (int)msg[msgctr + 3];
                    int wild = (int)msg[msgctr + 4];

                    if (wild == 1)
                    {
                        cd = new Card(CardNumber.Wild, (CardColor)col, id);
                        cd.Number = (CardNumber)num;
                    }
                    else
                    {
                        cd = new Card((CardNumber)num, (CardColor)col, id);
                    }

                    g.AddCard(cd);
                    msgctr = msgctr + 5;
                }

                _table.AddGroup(g);
            }
        }

        public TableMessage(Table t) : base()
        {
            //figure out length
            short length = 2;
            for (int gctr = 0; gctr < t.Count; gctr++)
            {
                length += 4; //4 bytes for each group
                length += (short)(t.ReadGroup(gctr).Count * 5); //5 bytes for each card!
            }


            _table = t;


            msg = new byte[length];
            msg[0] = (byte)pxMessages.Table;
            msg[1] = (byte)t.Count;
            short ptr = 2;

            int cardctr = 0;
            Group g = null;

            for (int gctr = 0; gctr < t.Count; gctr++)
            {
                cardctr = 0;
                g = t.ReadGroup(gctr);

                msg[ptr] = (byte)(g.Id);
                msg[ptr + 1] = (byte)(g.GroupType);
                msg[ptr + 2] = (byte)(g.CardsRequired);
                msg[ptr + 3] = (byte)(g.Count);
                ptr += 4;

                foreach (Card c in g)
                {
                    cardctr++;
                    msg[ptr] = (byte)(c.Id >> 8);
                    msg[ptr + 1] = (byte)(c.Id);
                    msg[ptr + 2] = (byte)(c.Color);
                    msg[ptr + 3] = (byte)(c.Number);

                    if (c.Wild) msg[ptr + 4] = (byte)1;
                    else msg[ptr + 4] = (byte)0;

                    ptr += 5;

                }
            }
        }


        public override string ToString()
        {
            string retval = "Table update, ";
            retval += _table.Count + " group(s) on the table : ";
            for (int ctr = 0; ctr < _table.Count; ctr++)
            {
                Group g = _table.ReadGroup(ctr);
                retval += g.ToString(false) + "; " ;
            }
            return retval;
        }

        public void Validate(GameRules rules)
        {
            ExceptionHelpers.CheckNotNull(rules, "rules");
            if (msg[0] != (byte)pxMessages.Table)
            {
                throw new BadMessageException("Table Message : Message length is invalid");
            }
            if (msg.Length < 2)
            {
                throw new BadMessageException("Table Message : Message does not start with the proper type");
            }

            int tmp;
            tmp = (int)msg[1];
            if (tmp < 0)
            {
                throw new BadMessageException("Table Message : Number of groups field is invalid");
            }
            int num_groups = tmp;
            int pos = 2;
            for (int ctr = 0; ctr < num_groups; ctr++)
            {
                tmp = (int)msg[pos + 1];
                if ((tmp < rules.MinimumGroupType) || (tmp > rules.MaximumGroupType))
                {
                    throw new BadMessageException("Table Message : Group type field is invalid");
                }
                tmp = (int)msg[pos + 2];
                if ((tmp < 0) || (tmp > rules.HandCards))
                {
                    throw new BadMessageException("Table Message : Cards required field is invalid");
                }
                tmp = (int)msg[pos + 3];
                if ((tmp < 0) || (tmp > rules.DeckCards))
                {
                    throw new BadMessageException("Table Message : Cards in group field is invalid");
                }
                int num_cards = tmp;
                pos += 4;
                for (int ctr2 = 0; ctr2 < num_cards; ctr2++)
                {
                    tmp = (int)msg[pos];
                    tmp <<= 8;
                    tmp += (int)msg[pos + 1];
                    if ((tmp < 0) || (tmp > rules.DeckCards))
                    {
                        throw new BadMessageException("Table Message : Card ID field is invalid");
                    }
                    tmp = (int)msg[pos + 2];
                    if ((tmp < (int)rules.MinimumCardColor) || (tmp > (int)rules.MaximumCardColor))
                    {
                        throw new BadMessageException("Table Message : Card color field is invalid");
                    }
                    tmp = (int)msg[pos + 3];
                    if ((tmp < (int)rules.MinimumCardNumber) || (tmp > (int)rules.MaximumCardNumber))
                    {
                        throw new BadMessageException("Table Message : Card number field is invalid");
                    }
                    tmp = (int)msg[pos + 4];
                    if ((tmp != 0) && (tmp != 1))
                    {
                        throw new BadMessageException("Table Message : Card wild field is invalid");
                    }
                    pos += 5;
                }
            }
        }
    }
}
