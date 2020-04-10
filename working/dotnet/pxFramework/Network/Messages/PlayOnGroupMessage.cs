using System;
using System.Collections.Generic;
using System.Text;
using PhazeX.Options;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /* play on group message
     * 
     * NAME                         BYTES
     * type                         1
     * group ID                     1
     * card ID                      2
     * card color                   1
     * card number                  1
     * card iswild                  1
     */
    public class PlayOnGroupMessage : Message
    {
        private int _groupid;
        public int GroupId { get { return _groupid; } }

        private Card _card;
        public Card CardObj { get { return _card; } }

        public PlayOnGroupMessage(byte [] b, GameRules rules) : base(b)
        {
            Validate(rules);
            _groupid = (int)msg[1];
            int id = (int)msg[2];
            id <<= 8;
            id += (int)msg[3];
            int col = (int)msg[4];
            int num = (int)msg[5];
            int wild = (int)msg[6];
            if (wild == 1)
            {
                _card = new Card(CardNumber.Wild, (CardColor)col, id);
                _card.Number = (CardNumber)num;
            }
            else
            {
                _card = new Card((CardNumber)num, (CardColor)col, id);
            }

        }

        public PlayOnGroupMessage(Card c, Group g) : base()
        {
            _groupid = g.Id;
            _card = c;

            msg = new byte[7];
            msg[0] = (byte)pxMessages.PlayOnGroup;
            msg[1] = (byte)g.Id;
            msg[2] = (byte)(c.Id >> 8);
            msg[3] = (byte)(c.Id);
            msg[4] = (byte)c.Color;
            msg[5] = (byte)c.Number;
            if (c.Wild)
            {
                msg[6] = (byte)1;
            }
            else
            {
                msg[6] = (byte)0;
            }
        }

        public override string ToString()
        {
            return "Play " + _card.ToString() + " on group " + _groupid;
        }

        public void Validate(GameRules rules)
        {
            if (msg.Length != 7)
            {
                throw new BadMessageException("Play on group Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.PlayOnGroup)
            {
                throw new BadMessageException("Play on group Message : Message does not start with the proper type");
            }

            int tmp;
            tmp = (int)msg[2];
            tmp <<= 8;
            tmp += (int)msg[3];
            if ((tmp < 0) || (tmp >= rules.DeckCards))
            {
                throw new BadMessageException("Play on group Message : Card ID field is invalid");
            }
            tmp = (int)msg[4];
            if ((tmp < (int)rules.MinimumCardColor) || (tmp > (int)rules.MaximumCardColor))
            {
                throw new BadMessageException("Play on group Message : Card color field is invalid");
            }
            tmp = (int)msg[5];
            if ((tmp < (int)rules.MinimumCardNumber) || (tmp > (int)rules.MaximumCardNumber))
            {
                throw new BadMessageException("Play on group Message : Card number field is invalid");
            }
            tmp = (int)msg[6];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Play on group Message : Card wild field is invalid");
            }
        }
    }
}
