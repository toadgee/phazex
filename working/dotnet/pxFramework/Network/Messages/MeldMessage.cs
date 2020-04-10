using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;
using PhazeX.Options;
using PhazeX.Helpers;

namespace PhazeX.Network.Messages
{
    /* meld message
     * 
     * NAME                        BYTES
     * type                        1
     * phaze number                1
     * number of groups            1
     * group kind                  1 * 
     * group cards required        1 *
     * group cards                 1 *
     * card ID                     2 **
     * card color                  1 **
     * card number                 1 **
     * card isWild                 1 **
     */
    public class MeldMessage : Message
    {
        public MeldMessage(List<Group> groups, int phazeNum)
        {
            short length = 3;
            foreach (Group g in groups)
            {
                length += 3;
                length += (short)(g.Count * 5);
            }

            msg = new byte[length];
            msg[0] = (byte)pxMessages.Meld;
            msg[1] = (byte)phazeNum;
            msg[2] = (byte)groups.Count;

            short ptr = 3;
            foreach (Group g in groups)
            {
                msg[ptr] = (byte)g.GroupType;
                msg[ptr + 1] = (byte)g.CardsRequired;
                msg[ptr + 2] = (byte)g.Count;
                ptr += 3;

                foreach (Card c in g)
                {
                    msg[ptr] = (byte)(c.Id >> 8);
                    msg[ptr + 1] = (byte)(c.Id);
                    msg[ptr + 2] = (byte)c.Color;
                    msg[ptr + 3] = (byte)c.Number;
                    if (c.Wild) msg[ptr + 4] = (byte)1;
                    else msg[ptr + 4] = (byte)0;
                    
                    ptr += 5;
                }
            }
        }

        private List<Group> _groups;
        public List<Group> Groups { get { return _groups; } }
        private int _phazeNumber;
        public int PhazeNumber { get { return _phazeNumber; } }

        public MeldMessage(byte [] b, GameRules rules) : base(b)
        {
            Validate(rules);
            _phazeNumber = (int)msg[1];
            Group g = null;
            Card c;
            int gk, gcr, gc;
            int ptr = 3;
            _groups = new List<Group>((int)msg[2]);
            for (int ctr = 0; ctr < (int)msg[2]; ctr++)
            {
                gk = (int)msg[ptr];
                gcr = (int)msg[ptr + 1];
                gc = (int)msg[ptr + 2];

                g = Group.Create((GroupType)gk, gcr);

                ptr += 3;

                for (int ctr2 = 0; ctr2 < gc; ctr2++)
                {
                    int id = (int)msg[ptr];
                    id <<= 8;
                    id += (int)msg[ptr + 1];
                    int col = (int)msg[ptr + 2];
                    int num = (int)msg[ptr + 3];
                    int wild = (int)msg[ptr + 4];

                    if (wild == 1)
                    {
                        c = new Card(CardNumber.Wild, (CardColor)col, id);
                        c.Number = (CardNumber)num;
                    }
                    else
                    {
                        c = new Card((CardNumber)num, (CardColor)col, id);
                    }
                    g.AddCard(c);
                    ptr += 5;
                }
                _groups.Add(g);
            }
        }

        public override string ToString()
        {
            string retval = "";
            foreach (Group g in _groups)
            {
                if (retval != "") retval += "; ";
                retval += g.ToString();
                retval += " : ";
                foreach (Card c in g)
                {
                    retval += c.ToString() + " ";
                }
            }

            return "Meld Message (" + _groups.Count + ") : " + retval;
        }

        public void Validate(GameRules rules)
        {
            if (msg.Length < 3)
            {
                throw new BadMessageException("Meld Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.Meld)
            {
                throw new BadMessageException("Meld Message : Message does not start with the proper type");
            }
            int tmp;
            tmp = (int)msg[2];
            if ((tmp < 1) || (tmp > rules.HandCards))
            {
                throw new BadMessageException("Meld Message : Number of groups field is invalid");
            }

            int num_groups = tmp;
            int pos = 3;
            for (int ctr = 0; ctr < num_groups; ctr++)
            {
                tmp = (int)msg[pos];
                if ((tmp < rules.MinimumGroupType) || (tmp > rules.MaximumGroupType))
                {
                    throw new BadMessageException("Meld Message : Group type field is invalid");
                }
                tmp = (int)msg[pos + 1];
                if ((tmp < 0) || (tmp > rules.HandCards))
                {
                    throw new BadMessageException("Meld Message : Cards required field is invalid");
                }
                tmp = (int)msg[pos + 2];
                if ((tmp < 0) || (tmp > rules.HandCards))
                {
                    throw new BadMessageException("Meld Message : Cards in group field is invalid");
                }
                pos += 3;
                int num_cards = tmp;
                for (int ctr2 = 0; ctr2 < num_cards; ctr2++)
                {
                    tmp = (int)msg[pos];
                    tmp <<= 8;
                    tmp += (int)msg[pos + 1];
                    if ((tmp < 0) || (tmp > rules.DeckCards))
                    {
                        throw new BadMessageException("Meld Message : Card ID field is invalid");
                    }

                    tmp = (int)msg[pos + 2];
                    if ((tmp < (int)rules.MinimumCardColor) || (tmp > (int)rules.MaximumCardColor))
                    {
                        throw new BadMessageException("Meld Message : Card color field is invalid");
                    }

                    tmp = (int)msg[pos + 3];
                    if ((tmp < (int)rules.MinimumCardNumber) || (tmp > (int)rules.MaximumCardNumber))
                    {
                        throw new BadMessageException("Meld Message : Card number field is invalid " + tmp);
                    }

                    tmp = (int)msg[pos + 4];
                    if ((tmp != 0) && (tmp != 1))
                    {
                        throw new BadMessageException("Meld Message : Card wild field is invalid");
                    }

                    pos += 5;
                }
            }
        }
    }
}
