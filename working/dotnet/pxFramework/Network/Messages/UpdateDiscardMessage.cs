using System;
using System.Collections.Generic;
using System.Text;
using PhazeX.Options;
using PhazeX;

namespace PhazeX.Network.Messages
{
    /* update discard message
     * 
     * NAME							BYTES
     * type							1
     * valid card                   1     0 if null, 1 if not
     * card color					1
     * card number					1
     * card iswild					1
     * card ID						2
     * discarded					1     1 if discarded (new discard), 0 if took discard
     * user id						1
     */
    public class UpdateDiscardMessage : Message
    {
        public delegate void UpdateDiscardMessageDelegate(Player p, bool discarded, Card newdiscard);

        private Card _card;
        public Card CardObj { get { return _card; } }

        private int _playerId;
        public int PlayerId { get { return _playerId; } }

        private bool _discarded;
        public bool Discarded { get { return _discarded; } }

        public UpdateDiscardMessage(byte [] b, int [] ids, GameRules rules) : base(b)
        {
            Validate(ids, rules);
            if ((int)msg[1] == 0) _card = null;
            else
            {
                int id = (int)msg[5];
                id <<= 8;
                id += (int)msg[6];

                int col = (int)msg[2];
                int num = (int)msg[3];
                int wild = (int)msg[4];

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
            _discarded = ((int)msg[7] == 1);
            _playerId = (int)msg[8];
        }

        public UpdateDiscardMessage(Card c, Player p, bool discarded) : base()
        {
            _card = c;
            if (p != null) _playerId = p.PlayerID;
            else _playerId = 254;
            _discarded = discarded;

            msg = new byte[9];
            msg[0] = (byte)pxMessages.UpdateDiscard;

            if (c == null)
            {
                // create an empty packet -- just make sure we decode it correctly on the other end!
                msg[1] = (byte)0;
                msg[2] = (byte)0;
                msg[3] = (byte)0;
                msg[4] = (byte)0;
                msg[5] = (byte)0;
                msg[6] = (byte)0;
            }
            else
            {
                msg[1] = (byte)1;
                msg[2] = (byte)(c.Color);
                msg[3] = (byte)(c.Number);
                if (c.Wild) msg[4] = (byte)1;
                else msg[4] = (byte)0;
                msg[5] = (byte)(c.Id >> 8);
                msg[6] = (byte)(c.Id);
            }

            if (discarded) msg[7] = (byte)1;
            else msg[7] = (byte)0;
            if (p != null) msg[8] = (byte)(p.PlayerID);
            else msg[8] = (byte)254;
        }



        public override string ToString()
        {
            string retval = "New discard : ";
            if (_card == null) retval += "empty";
            else retval += _card.ToString();
            retval += " [" + _playerId + "]";
            return retval;
        }

        public void Validate(int [] ids, GameRules rules)
        {
            if (msg.Length != 9)
            {
                throw new BadMessageException("Update Discard Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.UpdateDiscard)
            {
                throw new BadMessageException("Update Discard Message : Message does not start with the proper type");
            }
            int tmp;
            tmp = (int)msg[1];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Update Discard Message : Valid card field is invalid");
            }
            tmp = (int)msg[2];
            if ((tmp < (int)rules.MinimumCardColor) || (tmp > (int)rules.MaximumCardColor))
            {
                throw new BadMessageException("Update Discard Message : Card color field is invalid");
            }
            tmp = (int)msg[3];
            if ((tmp < (int)rules.MinimumCardNumber) || (tmp > (int)rules.MaximumCardNumber))
            {
                throw new BadMessageException("Update Discard Message : Card number field is invalid");
            }
            tmp = (int)msg[4];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Update Discard Message : Card wild field is invalid");
            }
            tmp = (int)msg[5];
            tmp <<= 8;
            tmp += (int)msg[6];
            if ((tmp < 0) || (tmp > rules.DeckCards))
            {
                throw new BadMessageException("Update Discard Message : Card ID field is invalid");
            }

            tmp = (int)msg[7];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Update Discard Message : Card discarded field is invalid");
            }

            tmp = (int)msg[8];
            if (tmp != 254)
            {
                bool found = false;
                for (int ctr = 0; ctr < ids.Length; ctr++)
                {
                    if (ids[ctr] == tmp) found = true;
                }
                if (!found)
                {
                    throw new BadMessageException("Update Discard Message : Player ID field is invalid!");
                }
            }
        }
    }
}
