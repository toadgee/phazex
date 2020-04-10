
using System;
namespace PhazeX.Options
{
	public class GameRules
	{	
		public GameRules()
		{
			// sets the default game rules
            this.PhazeRules = new PhazeRules();
            this.MinimumPlayers = 2;
            this.MaximumPlayers = 8;
            this.HandCards = 10;
			this.DeckCards = 108;
			this.MinimumCardNumber = CardNumber.Wild;
            this.MinimumCardColor = CardColor.Red;
            this.MaximumCardNumber = CardNumber.Skip;
            this.MaximumCardColor = CardColor.Blue;
            this.MinimumGroupType = 0;
            this.MaximumGroupType = 3;

            int numberSize = (int)(this.MaximumCardNumber - this.MinimumCardNumber + 1);
            int colorSize = (int)(this.MaximumCardColor - this.MinimumCardColor + 1);

			this.CardPoints = new int[numberSize];
			this.DeckDistribution = new int [numberSize][];
            for (int ctr = (int)this.MinimumCardNumber; ctr <= (int)this.MaximumCardNumber; ctr++)
			{
                this.DeckDistribution[ctr] = new int[colorSize];
			}

            for (int number = (int)this.MinimumCardNumber; number <= (int)this.MaximumCardNumber; number++)
			{
                switch (number)
                {
                    case (int)CardNumber.Wild:
                        this.CardPoints[number] = 50;
                        break;
                    case (int)CardNumber.Skip:
                        this.CardPoints[number] = 25;
                        break;
                    case (int)CardNumber.Draw:
                    case (int)CardNumber.Reverse:
                        this.CardPoints[number] = 100;
                        break;
                    default:
                        this.CardPoints[number] = number;
                        break;
                }

                for (int color = (int)this.MinimumCardColor; color <= (int)this.MaximumCardColor; color++)
                {
                    switch (number)
                    {
                        case (int)CardNumber.Wild:
                        case (int)CardNumber.Card1:
                        case (int)CardNumber.Card2:
                        case (int)CardNumber.Card3:
                        case (int)CardNumber.Card4:
                        case (int)CardNumber.Card5:
                        case (int)CardNumber.Card6:
                        case (int)CardNumber.Card7:
                        case (int)CardNumber.Card8:
                        case (int)CardNumber.Card9:
                        case (int)CardNumber.Card10:
                        case (int)CardNumber.Card11:
                        case (int)CardNumber.Card12:
                            this.DeckDistribution[number][color] = 2;
                            break;
                        case (int)CardNumber.Skip:
                            if (color == (int)CardColor.Blue)
                            {
                                this.DeckDistribution[number][color] = 4;
                            }
                            else
                            {
                                this.DeckDistribution[number][color] = 0;
                            }

                            break;

                        default:
                            this.DeckDistribution[number][color] = 0;
                            break;
                    }
				}
			}

            this.SkipCardsEnabled = true;
            this.DrawCardsEnabled = false;
            this.ReverseCardsEnabled = false;
            this.WildCardsEnabled = true;
            this.ExtraSetPlayable = false;
            this.AllowWildColor = false;
            this.SkipAnybody = true;
			this.SkipOncePerHand = false;
		}

        public PhazeRules PhazeRules
        {
            get;
            set;
        }

        public int MinimumPlayers
        {
            get;
            set;
        }

        public int MaximumPlayers
        {
            get;
            set;
        }

        public int DeckCards
        {
            get;
            set;
        }

        public int HandCards
        {
            get;
            set;
        }

        public CardNumber MinimumCardNumber
        {
            get;
            set;
        }

        public CardNumber MaximumCardNumber
        {
            get;
            set;
        }

        public CardColor MinimumCardColor
        {
            get;
            set;
        }

        public CardColor MaximumCardColor
        {
            get;
            set;
        }

        public int[] CardPoints
        {
            get;
            set;
        }

        public int[][] DeckDistribution
        {
            get;
            set;
        }

        public int MinimumGroupType
        {
            get;
            set;
        }

        public int MaximumGroupType
        {
            get;
            set;
        }

        public bool SkipCardsEnabled
        {
            get;
            set;
        }

        public bool DrawCardsEnabled
        {
            get;
            set;
        }

        public bool ReverseCardsEnabled
        {
            get;
            set;
        }

        public bool WildCardsEnabled
        {
            get;
            set;
        }

        public bool ExtraSetPlayable
        {
            get;
            set;
        }

        public bool AllowWildColor
        {
            get;
            set;
        }

        public bool SkipAnybody
        {
            get;
            set;
        }

        public bool SkipOncePerHand
        {
            get;
            set;
        }

        public int MaximumCardPoints()
        {
            int i = 0;
            int j;
            for (int ctr = (int)this.MinimumCardNumber; ctr <= (int)this.MaximumCardNumber; ctr++)
            {
                j = GetCardPointValue((CardNumber)ctr);
                if (i < j) i = j;
            }

            return i;
        }

		public int GetCardPointValue(CardNumber cardnum)
		{
			int ret_val = 0;

            if ((int)cardnum <= (int)this.MaximumCardNumber)
			{
                ret_val = this.CardPoints[(int)cardnum];
			}
			
			return ret_val;
		}
		
		public int CardsCountAllColors(CardNumber cardnum)
		{
			int i = 0;
            for (int ctr = (int)this.MinimumCardColor; ctr <= (int)this.MaximumCardColor; ctr++)
			{
				i += CardsCount(cardnum, (CardColor)ctr);
			}
			return i;
		}
		public int CardsCountAllNumbers(CardColor cardcol)
		{
			int i = 0;
            for (int ctr = (int)this.MinimumCardNumber; ctr <= (int)this.MaximumCardNumber; ctr++)
			{
                i += CardsCount((CardNumber)ctr, cardcol);
			}
			return i;
		}

		public int CardsCount(CardNumber cardnum, CardColor cardcol)
		{
			int ret_val = 0;
            if (((int)cardnum <= (int)this.MaximumCardNumber) && ((int)cardcol <= (int)this.MaximumCardColor))
			{
                ret_val = this.DeckDistribution[(int)cardnum][(int)cardcol];
			}
			return ret_val;
		}

        public int GetMaxCardPoints()
        {
            int max_points = 0;
            for (int num = (int)this.MinimumCardNumber; num < (int)this.MaximumCardNumber; num++)
            {
                if (this.CardPoints[num] > max_points)
                {
                    max_points = this.CardPoints[num];
                }
            }
            if ((this.CardPoints[(int)CardNumber.Wild] > max_points) && (this.WildCardsEnabled))
            {
                max_points = this.CardPoints[(int)CardNumber.Wild];
            }
            if ((this.CardPoints[(int)CardNumber.Skip] > max_points) && (this.SkipCardsEnabled))
            {
                max_points = this.CardPoints[(int)CardNumber.Skip];
            }
            if (this.DrawCardsEnabled)
            {
                if (this.CardPoints[(int)CardNumber.Draw] > max_points)
                {
                    max_points = this.CardPoints[(int)CardNumber.Draw];
                }
            }

            if (this.ReverseCardsEnabled)
            {
                if (this.CardPoints[(int)CardNumber.Reverse] > max_points)
                {
                    max_points = this.CardPoints[(int)CardNumber.Reverse];
                }
            }

            return max_points;
        }

        public int HowManyCards(CardNumber num)
        {
            int i = 0;
            for (int col = (int)this.MinimumCardColor; col <= (int)this.MaximumCardColor; col++)
            {
                i += this.DeckDistribution[(int)num][col];
            }

            return i;
        }
    }
}
