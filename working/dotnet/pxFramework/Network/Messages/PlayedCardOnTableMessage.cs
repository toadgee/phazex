using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;
using PhazeX.Helpers;
using PhazeX.Options;

namespace PhazeX.Network.Messages
{
    /*  played card on table message
	 * 
	 * NAME							BYTES
	 * type							1
	 * player id					1
	 * group id                     1
	 * group kind					1
	 * group cards required			1
	 * card color					1
	 * card number					1
	 * card iswild					1 
	 * card id						2
	 *
	 */
    public class PlayedCardOnTableMessage : Message
    {
        public delegate void PlayedCardOnTableMessageDelegate(Player p, Group g, Card c);
        public PlayedCardOnTableMessage(int player_id, Group g, Card c) : base()
        {
            _playerId = player_id;
            _group = g;
            _card = c;

            msg = new byte[10];
            msg[0] = (byte)pxMessages.PlayedCardOnTable;
            msg[1] = (byte)player_id;
            msg[2] = (byte)g.Id;
            msg[3] = (byte)g.GroupType;
            msg[4] = (byte)g.CardsRequired;
            msg[5] = (byte)c.Color;
            msg[6] = (byte)c.Number;
            
            if (c.Wild) msg[7] = (byte)1;
            else msg[7] = (byte)0;

            msg[8] = (byte)(c.Id >> 8);
            msg[9] = (byte)(c.Id);
        }

        private Group _group;
        public Group GroupObj { get { return _group; } }
        private Card _card;
        public Card CardObj { get { return _card; } }
        private int _playerId;
        public int PlayerId { get { return _playerId; } }

        public PlayedCardOnTableMessage(byte [] b, int [] ids, GameRules rules) : base(b)
        {
            Validate(ids, rules);
            _playerId = (int)msg[1];
            int g_id_ptr = 2;
            int type_ptr = 3;
            int cr_ptr = 4;
            int num_ptr = 6;
            int col_ptr = 5;
            int wild_ptr = 7;
            int c_id_ptr = 8; //this will start a 2 byte section

            int type = (int)msg[type_ptr];
            int id = (int)msg[g_id_ptr];
            int cardsreq = (int)msg[cr_ptr];
            _group = Group.Create((GroupType)type, cardsreq);
            _group.Id = id;
            
            id = (((int)msg[c_id_ptr]) << 8) + ((int)msg[c_id_ptr + 1]);
            if ((int)msg[wild_ptr] == 1)
            {
                _card = new Card(CardNumber.Wild, (CardColor)msg[col_ptr], id);
                _card.Number = (CardNumber)msg[num_ptr];
            }
            else
            {
                _card = new Card((CardNumber)msg[num_ptr], (CardColor)msg[col_ptr], id);
            }
        }

        public override string ToString()
        {
            return "Played Card on Table";
        }

        public void Validate(int [] ids, GameRules rules)
        {
            if (msg.Length != 10)
            {
                throw new BadMessageException("Played Card on Table Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.PlayedCardOnTable)
            {
                throw new BadMessageException("Played Card on Table Message : Message does not start with the proper type");
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
                throw new BadMessageException("Played Card on Table Message : Unknown player ID");
            }

            tmp = (int)msg[3];
            if ((tmp < rules.MinimumGroupType) || (tmp > rules.MaximumGroupType))
            {
                throw new BadMessageException("Played Card on Table Message : Group kind field is invalid");
            }

            tmp = (int)msg[4];
            if ((tmp < 1) || (tmp > rules.HandCards))
            {
                throw new BadMessageException("Played Card on Table Message : Group cards required field is invalid");
            }

            tmp = (int)msg[5];
            if ((tmp < (int)rules.MinimumCardColor) || (tmp > (int)rules.MaximumCardColor))
            {
                throw new BadMessageException("Played Card on Table Message : Card color field is invalid");
            }

            tmp = (int)msg[6];
            if ((tmp < (int)rules.MinimumCardNumber) || (tmp > (int)rules.MaximumCardNumber))
            {
                throw new BadMessageException("Played Card on Table Message : Card number field is invalid");
            }

            tmp = (int)msg[7];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Played Card on Table Message : Card wild status field is invalid");
            }

            tmp = (int)msg[8];
            tmp <<= 8;
            tmp += (int)msg[9];
            if ((tmp < 0) || (tmp > rules.DeckCards))
            {
                throw new BadMessageException("Played Card on Table Message : Card ID field is invalid");
            }
        }
    }
}
