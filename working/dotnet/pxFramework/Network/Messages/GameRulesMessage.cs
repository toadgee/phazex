using System;
using System.Collections.Generic;
using System.Text;
using PhazeX.Options;
using PhazeX;
using System.Collections;
using PhazeX.Helpers;

namespace PhazeX.Network.Messages
{
    /* game rules message
	 *
	 * NAME						    BYTES		POS
	 * type                         1			0
	 * minimum players				1			1
	 * maximum players				1			2
	 * hand cards					1			3
	 * deck cards					1			4
	 * minimum card number			1			5
	 * maximum card number			1			6
	 * minimum card color			1			7
	 * maximum card color           1			8
	 * minimum group type			1			9
	 * maximum group type			1			10
	 * skip cards enabled			1			11
	 * draw cards enabled			1			12
	 * reverse cards enabled        1			13
	 * wild cards enabled			1			14
	 * extra set playable			1			15
	 * allow wild color				1			16
	 * skip anybody					1			17
	 * skip once per hand			1			18
	 * number of phazes             1           19
	 * number of groups in phaze    1 *         ...
	 * group kind                   1 **        ...
	 * cards in group               1 **        ...
	 * card points					2 *			...
	 */
    public class GameRulesMessage : Message
    {
        public delegate void GameRulesMessageDelegate();
        public GameRulesMessage(GameRules gr) : base()
        {
            ExceptionHelpers.CheckNotNull(gr, "gr");

            short length = 20;
            for (int i = 0; i < gr.PhazeRules.Count(); i++)
            {
                length += 1; //group kind
                length += (short)(gr.PhazeRules.Phaze(i).Count() * 2);
            }
            length += (short)(gr.CardPoints.Length * 2); //2 for each card

            msg = new byte[length];
            msg[0] = (byte)pxMessages.GameRules;
            msg[1] = (byte)(gr.MinimumPlayers);
            msg[2] = (byte)(gr.MaximumPlayers);
            msg[3] = (byte)(gr.HandCards);
            msg[4] = (byte)(gr.DeckCards);
            msg[5] = (byte)(gr.MinimumCardNumber);
            msg[6] = (byte)(gr.MaximumCardNumber);
            msg[7] = (byte)(gr.MinimumCardColor);
            msg[8] = (byte)(gr.MaximumCardColor);
            msg[9] = (byte)(gr.MinimumGroupType);
            msg[10] = (byte)(gr.MaximumGroupType);

            if (gr.SkipCardsEnabled) msg[11] = (byte)1;
            else msg[11] = (byte)0;
            if (gr.DrawCardsEnabled) msg[12] = (byte)1;
            else msg[12] = (byte)0;
            if (gr.ReverseCardsEnabled) msg[13] = (byte)1;
            else msg[13] = (byte)0;
            if (gr.WildCardsEnabled) msg[14] = (byte)1;
            else msg[14] = (byte)0;
            if (gr.ExtraSetPlayable) msg[15] = (byte)1;
            else msg[15] = (byte)0;
            if (gr.AllowWildColor) msg[16] = (byte)1;
            else msg[16] = (byte)0;
            if (gr.SkipAnybody) msg[17] = (byte)1;
            else msg[17] = (byte)0;
            if (gr.SkipOncePerHand) msg[18] = (byte)1;
            else msg[18] = (byte)0;

            msg[19] = (byte)gr.PhazeRules.Count();

            short ptr = 20;

            for (int ctr = 0; ctr < gr.PhazeRules.Count(); ctr++)
            {
                PhazeRule pr = gr.PhazeRules.Phaze(ctr);
                msg[ptr] = (byte)pr.Count();
                ptr += 1;
                for (int ctr2 = 0; ctr2 < pr.Count(); ctr2++)
                {
                    Group g = pr.GetGroup(ctr2);
                    msg[ptr] = (byte)g.GroupType;
                    msg[ptr + 1] = (byte)g.CardsRequired;
                    ptr += 2;
                }
            }

            foreach (int i in gr.CardPoints)
            {
                msg[ptr] = (byte)(i >> 8);
                msg[ptr + 1] = (byte)(i);
                ptr += 2;
            }
        }

        public GameRulesMessage(byte[] b)
            : base(b)
        {
            Validate();
            GameRules gr = new GameRules();
            gr.MinimumPlayers = (int)msg[1];
            gr.MaximumPlayers = (int)msg[2];
            gr.HandCards = (int)msg[3];
            gr.DeckCards = (int)msg[4];
            gr.MinimumCardNumber = (CardNumber)msg[5];
            gr.MaximumCardNumber = (CardNumber)msg[6];
            gr.MinimumCardColor = (CardColor)msg[7];
            gr.MaximumCardColor = (CardColor)msg[8];
            gr.MinimumGroupType = (int)msg[9];
            gr.MaximumGroupType = (int)msg[10];
            gr.SkipCardsEnabled = ((int)msg[11] == 1);
            gr.DrawCardsEnabled = ((int)msg[12] == 1);
            gr.ReverseCardsEnabled = ((int)msg[13] == 1);
            gr.WildCardsEnabled = ((int)msg[14] == 1);
            gr.ExtraSetPlayable = ((int)msg[15] == 1);
            gr.AllowWildColor = ((int)msg[16] == 1);
            gr.SkipAnybody = ((int)msg[17] == 1);
            gr.SkipOncePerHand = ((int)msg[18] == 1);


            List<PhazeRule> prs = new List<PhazeRule>();
            int numberOfPhazes = (int)msg[19];
            int pos = 20;
            for (int ctr = 0; ctr < numberOfPhazes; ctr++)
            {
                int gmax = (int)msg[pos];
                pos += 1;
                List<Group> apr = new List<Group>();
                for (int gctr = 0; gctr < gmax; gctr++)
                {
                    apr.Add(Group.Create((GroupType)msg[pos], (int)msg[pos + 1]));
                    pos += 2;
                }
                prs.Add(new PhazeRule(apr, ctr));
            }

            gr.PhazeRules.ReplacePhazeRules(prs);

            int[] points = gr.CardPoints;
            for
            (
                int ctr = (int)gr.MinimumCardNumber;
                ctr <= (int)gr.MaximumCardNumber;
                ctr++
            )
            {
                points[ctr] = (int)msg[pos];
                points[ctr] <<= 8;
                points[ctr] += (int)msg[pos + 1];

                pos += 2;
            }

            gr.CardPoints = points;

            this.GameRules = gr;
        }

        public GameRules GameRules
        {
            get;
            set;
        }

        public override string ToString()
        {
            return "Game Rules";
        }

        public void Validate()
        {
            if (msg.Length < 25)
            {
                throw new BadMessageException("Game Rules Message : Message length is invalid");
            }

            if (msg[0] != (byte)pxMessages.GameRules)
            {
                throw new BadMessageException("Game Rules Message : Message does not start with the proper type");
            }

            int tmp;
            tmp = (int)msg[1];
            if ((tmp < 2) || (tmp > 32))
            {
                throw new BadMessageException("Game Rules Message : Minimum Player field is invalid");
            }

            tmp = (int)msg[2];
            if ((tmp < 2) || (tmp > 32))
            {
                throw new BadMessageException("Game Rules Message : Maximum Player field is invalid");
            }

            int hc = (int)msg[3];
            int dc = (int)msg[4];
            if ((dc / hc) < tmp) //deck cards / hand cards < maximum_players
            {
                throw new BadMessageException("Game Rules Message : Hand cards and deck cards fields are invalid");
            }

            tmp = (int)msg[11];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Game Rules Message : Skip Cards enabled field is invalid");
            }

            tmp = (int)msg[12];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Game Rules Message : Draw Cards enabled field is invalid");
            }

            tmp = (int)msg[13];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Game Rules Message : Reverse Cards enabled field is invalid");
            }

            tmp = (int)msg[14];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Game Rules Message : Wild Cards enabled field is invalid");
            }

            tmp = (int)msg[15];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Game Rules Message : Extra set playable field is invalid");
            }

            tmp = (int)msg[16];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Game Rules Message : Allow wild color field is invalid");
            }

            tmp = (int)msg[17];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Game Rules Message : Skip anybody field is invalid");
            }

            tmp = (int)msg[18];
            if ((tmp != 0) && (tmp != 1))
            {
                throw new BadMessageException("Game Rules Message : Skip once per hand field is invalid");
            }

            tmp = (int)msg[19];
            if ((tmp < 1) || (tmp > 30))
            {
                throw new BadMessageException("Game Rules Message : Maximum Phaze Number is invalid");
            }

            int pos = 20;
            int hand_cards = (int)msg[3];
            int min_group_type = (int)msg[9];
            int max_group_type = (int)msg[10];
            int num_phazes = (int)msg[19];
            for (int ctr = 0; ctr < num_phazes; ctr++)
            {
                if (msg.Length <= pos)
                {
                    throw new BadMessageException("Game Rules Message : Message length is invalid");
                }

                int num_groups = (int)msg[pos];
                if (num_groups > hand_cards)
                {
                    throw new BadMessageException("Game Rules Message : Number of groups field is invalid");
                }

                pos += 1;
                for (int ctr2 = 0; ctr2 < num_groups; ctr2++)
                {
                    if (msg.Length <= pos)
                    {
                        throw new BadMessageException("Game Rules Message : Message length is invalid");
                    }

                    if (((int)msg[pos] < min_group_type) || ((int)msg[pos] > max_group_type))
                    {
                        throw new BadMessageException("Game Rules Message : Group type is invalid!");
                    }

                    pos += 1;
                    if (msg.Length <= pos)
                    {
                        throw new BadMessageException("Game Rules Message : Message length is invalid");
                    }

                    if (((int)msg[pos] < 1) || ((int)msg[pos] > hand_cards))
                    {
                        throw new BadMessageException("Game Rules Message : Cards in group is invalid");
                    }

                    pos += 1;
                }
            }


            int max_card_number = (int)msg[5];
            int max_card_color = (int)msg[6];
            for (int ctr = 0; ctr < max_card_number; ctr++)
            {
                for (int ctr2 = 0; ctr2 < max_card_color; ctr2++)
                {
                    if (msg.Length <= (pos + 1))
                    {
                        throw new BadMessageException("Gam Rules Message : Message length is invalid");
                    }
                    tmp = (int)msg[pos];
                    tmp <<= 8;
                    tmp += (int)msg[pos + 1];

                    if ((tmp < 0) || (tmp > 1000))
                    {
                        throw new BadMessageException("Game Rules Message : Card points field is invalid (" + tmp + ")");
                    }

                    pos += 2;
                }
            }
        }
    }
}
