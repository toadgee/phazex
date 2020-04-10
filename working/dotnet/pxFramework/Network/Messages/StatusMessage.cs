using System;
using System.Collections.Generic;
using System.Text;
using PhazeX;
using PhazeX.Options;
using PhazeX.Helpers;

namespace PhazeX.Network.Messages
{
    /* status message
	 *
	 * NAME                                                 BYTES
	 * type                                                 1
	 * player id                                            1
	 * completed phaze                                      1 (0 = no, 1 = yes)
	 * current phaze                                        1
	 * cards in hand                                        1
     * skips left                                           1
	 */
    public class StatusMessage : Message
    {
        public delegate void StatusMessageDelegate(Player p);

        private int _id;
        public int Id { get { return _id; } }
        private bool _madePhaze;
        public bool MadePhaze { get { return _madePhaze; } }
        private int _currentPhaze;
        public int CurrentPhaze { get { return _currentPhaze; } }
        private int _cardsInHand;
        public int CardsInHand { get { return _cardsInHand; } }
        private int _skipsLeft;
        public int SkipsLeft { get { return _skipsLeft; } }

        public StatusMessage(byte [] b, int[] ids, GameRules rules)
            : base(b)
        {
            Validate(ids, rules);
            _log = false;
            _id = (int)msg[1];
            _madePhaze = ((int)msg[2] == 1);
            _currentPhaze = (int)msg[3];
            _cardsInHand = (int)msg[4];
            _skipsLeft = (int)msg[5];
        }

        public StatusMessage(
            int player_id, bool made_phaze, int current_phaze
            , int cards_in_hand, int skips) : base()
        {
            _id = player_id;
            _madePhaze = made_phaze;
            _currentPhaze = current_phaze;
            _cardsInHand = cards_in_hand;
            _skipsLeft = skips;

            msg = new byte[6];
            msg[0] = (byte)pxMessages.Status;
            msg[1] = (byte)player_id;

            if (made_phaze) msg[2] = (byte)1;
            else msg[2] = (byte)0;

            msg[3] = (byte)current_phaze;
            msg[4] = (byte)cards_in_hand;
            msg[5] = (byte)skips;
        }

        public override string ToString()
        {
            return "Status Message [" + _id + "]";
        }

        private void Validate(int[] ids, GameRules rules)
        {
            if (msg.Length != 6)
            {
                throw new BadMessageException("Status Message : Message length is invalid");
            }
            if (msg[0] != (byte)pxMessages.Status)
            {
                throw new BadMessageException("Status Message : Message does not start with the proper type");
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
                throw new BadMessageException("Status Message : Unknown player ID");
            }

            tmp = (int)msg[2];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Status Message : Completed phaze field is invalid");
            }

            tmp = (int)msg[3];
            if ((tmp < 0) || (tmp > rules.PhazeRules.Count() - 1))
            {
                throw new BadMessageException("Status Message : Current phaze field is invalid");
            }

            tmp = (int)msg[4];
            if ((tmp < 0) || (tmp > (rules.HandCards + 1)))
            {
                throw new BadMessageException("Status Message : Cards in hand field is invalid (" + tmp + ") for player " + (int)msg[1]);
            }

            tmp = (int)msg[5];
            if ((tmp < 0) || (tmp > rules.HowManyCards(CardNumber.Skip)))
            {
                throw new BadMessageException("Status Message : Skips left is invalid", "0 : " + rules.HowManyCards(CardNumber.Skip), tmp.ToString());
            }
        }
    }
}
