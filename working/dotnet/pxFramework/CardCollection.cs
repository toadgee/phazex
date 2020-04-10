// this file has had the header file ported to OSX

namespace PhazeX
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using PhazeX.Helpers;
    using PhazeX.Options;

    public class CardCollection
    {
        public event EventHandler<EventArgs> ItemsChanged;

        protected List<Card> cards;

        /// <summary>
        /// Basic constructor
        /// </summary>
        public CardCollection()
        {
            this.cards = new List<Card>();
        }

        /// <summary>
        /// Constructor for passing in arrays of cards!
        /// </summary>
        /// <param name="cards">Array of cards to init with.</param>
        public CardCollection(IEnumerable<Card> cards)
        {
            this.cards = new List<Card>(cards);
        }

        /// <summary>
        /// Constructor for passing in initial capacity.
        /// </summary>
        /// <param name="capacity">Initial capacity</param>
        public CardCollection(int capacity)
        {
            this.cards = new List<Card>(capacity);
        }

        /// <summary>
        /// Gets the number of cards in the card collection.
        /// </summary>
        public int Count
        {
            get
            {
                return this.cards.Count;
            }
        }

        /// <summary>
        /// Gets the card at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The card.</returns>
        public Card this[int index]
        {
            get
            {
                return this.cards[index];
            }
        }

        private void OnItemsChanged()
        {
            EventHandler<EventArgs> ea = this.ItemsChanged;
            if (ea != null)
            {
                ea(this, new EventArgs());
            }
        }

        public void Add(Card card)
        {
            this.cards.Add(card);
            this.OnItemsChanged();
        }

        public void Insert(int index, Card card)
        {
            this.cards.Insert(index, card);
            this.OnItemsChanged();
        }

        public bool Remove(Card card)
        {
            bool retval = this.cards.Remove(card);
            this.OnItemsChanged();
            return retval;
        }

        public void RemoveAt(int index)
        {
            this.cards.RemoveAt(index);
            this.OnItemsChanged();
        }

        public void Clear()
        {
            this.cards.Clear();
            this.OnItemsChanged();
        }

        public void AddRange(CardCollection collection)
        {
            this.cards.AddRange(collection.cards);
            this.OnItemsChanged();
        }

        public void AddRange(IEnumerable<Card> collection)
        {
            this.cards.AddRange(collection);
            this.OnItemsChanged();
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return this.cards.GetEnumerator();
        }

        public void Reverse()
        {
            this.cards.Reverse();
            this.OnItemsChanged();
        }

        /// <summary>
        /// simply resets all the wilds in this collection
        /// to be CardNumber.WILD_CARD
        /// </summary>
        public void ResetWilds()
        {
            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this.cards[ctr];
                
                if (c.Wild)
                {
                    c.Number = CardNumber.Wild;
                }
            }
        }

        /// <summary>
        /// Returns the minimum numbered card number in this card collection.
        /// </summary>
        /// <returns>the minimum numbered card number</returns>
        public CardNumber MinimumCardNumber()
        {
            CardNumber min = CardNumber.Draw;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this[ctr];

                // if the card is a numbered card
                // and the value is less than min
                if ((c.IsNumberedCard()) && (c.Number < min))
                {
                    min = c.Number;
                }
            }

            return min;
        }

        /// <summary>
        /// Returns the maximum numbered card number in this card collection.
        /// </summary>
        /// <returns>the maximum numbered card number</returns>
        public CardNumber MaximumCardNumber()
        {
            CardNumber max = CardNumber.Card1;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this[ctr];

                // if the card is a numbered card
                // and it's greater than max (which by rules, no card can be -1)
                if ((c.IsNumberedCard()) && (c.Number > max))
                {
                    max = c.Number;
                }
            }

            return max;
        }

        /// <summary>
        /// Returns without removing the first card in the collection of cards. Can be null.
        /// </summary>
        /// <returns>The first card in the card collection.</returns>
        public Card ReadFirst()
        {
            if (this.Count == 0)
            {
                return null;
            }

            return this[0];
        }

        /// <summary>
        /// Counts the number of cards in the collection with the specified number.
        /// </summary>
        /// <param name="num">The number to look for</param>
        /// <returns>The number of cards in the card collection that have the same number</returns>
        public int CountCardNumber(CardNumber num)
        {
            int retval = 0;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                if (this[ctr].Number == num)
                {
                    retval++;
                }
            }

            return retval;
        }

        /// <summary>
        /// Counts the number of cards in the collection with the specified number but not color.
        /// </summary>
        /// <param name="num">Number to count</param>
        /// <param name="col">Color you don't want</param>
        /// <returns>How many exist that are num but NOT col</returns>
        public int CountCardNumberNotWithColor(CardNumber num, CardColor col)
        {
            int retval = 0;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this[ctr];

                if (c.Color != col && c.Number == num)
                {
                    retval++;
                }
            }

            return retval;
        }

        /// <summary>
        /// Counts the number of cards in the card collection with specified color.
        /// </summary>
        /// <param name="col">The color to look for</param>
        /// <returns>The number of cards in the card collection that are of the same color</returns>
        public int CountCardColor(CardColor col)
        {
            int retval = 0;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                if (this[ctr].Color == col)
                {
                    retval++;
                }
            }

            return retval;
        }

        /// <summary>
        /// Counts the number of cards in the collection with the specified color but NOT number
        /// </summary>
        /// <param name="color">Color to count</param>
        /// <param name="number">Number you don't want</param>
        /// <returns>How many exist that are num but NOT col</returns>
        public int CountCardColorNotWithNumber(CardColor color, CardNumber number)
        {
            int retval = 0;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this[ctr];

                if (c.Color == color && c.Number != number)
                {
                    retval++;
                }
            }

            return retval;
        }

        /// <summary>
        /// Order the card collection by color.
        /// </summary>
        public void OrderByColor(GameRules rules)
        {
            ExceptionHelpers.CheckNotNull(rules, "rules");

            CardCollection copy = this.Copy();
            this.Clear();

            int minCol = (int)rules.MinimumCardColor;
            int maxCol = (int)rules.MaximumCardColor;
            int minNum = (int)rules.MinimumCardNumber;
            int maxNum = (int)rules.MaximumCardNumber;

            for (int col = minCol; col <= maxCol; col++)
            {
                for (int num = minNum; num <= maxNum; num++)
                {
                    while (true)
                    {
                        Card c = copy.RemoveCardWithNumberAndColor((CardNumber)num, (CardColor)col);

                        if (c == null)
                        {
                            break;
                        }

                        this.Add(c);
                    }
                }
            }

            this.OnItemsChanged();
        }

        /// <summary>
        /// Orders the card collection by number.
        /// </summary>
        public void OrderByNumber(GameRules rules)
        {
            ExceptionHelpers.CheckNotNull(rules, "rules");

            CardCollection copy = this.Copy();
            this.Clear();

            int minNum = (int)rules.MinimumCardNumber;
            int maxNum = (int)rules.MaximumCardNumber;
            int minCol = (int)rules.MinimumCardColor;
            int maxCol = (int)rules.MaximumCardColor;

            for (int num = minNum; num <= maxNum; num++)
            {
                for (int col = minCol; col <= maxCol; col++)
                {
                    while (true)
                    {
                        Card c = copy.RemoveCardWithNumberAndColor((CardNumber)num, (CardColor)col);
                        
                        if (c == null)
                        {
                            break;
                        }

                        this.Add(c);
                    }
                }
            }

            this.OnItemsChanged();
        }

        /// <summary>
        /// Orders the card collection by points.
        /// </summary>
        public void OrderByPoints(GameRules rules)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines if the card collection contains a card with the specified ID.
        /// </summary>
        /// <param name="id">ID to look for</param>
        /// <returns>True if the card exists, false if otherwise.</returns>
        public bool HasCardWithId(int id)
        {
            bool retval = false;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                if (this[ctr].Id == id)
                {
                    retval = true;
                    break;
                }
            }

            return retval;
        }

        /// <summary>
        /// Removes the card with the specified ID. Returns the card removed (can be null).
        /// </summary>
        /// <param name="id">Card ID to look for and remove.</param>
        /// <returns>Card removed (can be null)</returns>
        public Card RemoveCardWithId(int id)
        {
            Card retval = null;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this[ctr];
                if (c.Id == id)
                {
                    retval = c;
                    this.Remove(c);
                    break;
                }
            }

            return retval;
        }

        /// <summary>
        /// Finds the first card found that has the number and color of the card passed. If null,
        /// returns null. This function actually uses the overloaded function num,col.
        /// </summary>
        /// <param name="card">Card to check</param>
        /// <returns>The card that was found.</returns>
        public Card FindCardWithNumberAndColor(Card card)
        {
            ExceptionHelpers.CheckNotNull(card, "card");

            return this.FindCardWithNumberAndColor(card.Number, card.Color);
        }

        public bool HasCardWithNumberAndColor(CardNumber num, CardColor col)
        {
            bool retval = false;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this[ctr];
                if (c.Color == col && c.Number == num)
                {
                    retval = true;
                    break;
                }
            }

            return retval;
        }

        public Card FindCardWithNumber(CardNumber num)
        {
            Card retval = null;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this[ctr];
                if (c.Number == num)
                {
                    retval = c;
                    break;
                }
            }

            return retval;
        }

        public Card FindCardWithNumberAndNotColor(CardNumber num, CardColor col)
        {
            Card retval = null;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this[ctr];
                if (c.Color != col && c.Number == num)
                {
                    retval = c;
                    break;
                }
            }

            return retval;
        }

        public Card FindCardWithId(int id)
        {
            Card retval = null;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this[ctr];
                if (c.Id == id)
                {
                    retval = c;
                    break;
                }
            }

            return retval;
        }

        public Card FindCardWithColor(CardColor col)
        {
            Card retval = null;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                if (this[ctr].Color == col)
                {
                    retval = this[ctr];
                    break;
                }
            }

            return retval;
        }

        public Card FindCardWithColorAndNotNumber(CardColor col, CardNumber num)
        {
            Card retval = null;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this[ctr];
                if (c.Color == col && c.Number != num)
                {
                    retval = c;
                    break;
                }
            }

            return retval;
        }

        /// <summary>
        /// Finds the first card found with the specified number and color
        /// </summary>
        /// <param name="num">Number</param>
        /// <param name="col">Color</param>
        /// <returns>Card found</returns>
        public Card FindCardWithNumberAndColor(CardNumber num, CardColor col)
        {
            Card retval = null;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this[ctr];
                if (c.Number == num && c.Color == col)
                {
                    retval = c;
                    break;
                }
            }

            return retval;
        }

        /// <summary>
        /// Removes the first card found with the specified number and color
        /// </summary>
        /// <param name="num">Number</param>
        /// <param name="col">Color</param>
        /// <returns>Card actually removed</returns>
        public Card RemoveCardWithNumberAndColor(CardNumber num, CardColor col)
        {
            Card toremove = this.FindCardWithNumberAndColor(num, col);

            if (toremove != null)
            {
                this.Remove(toremove);
            }

            return toremove;
        }

        /// <summary>
        /// Copies the card collection and returns the copy
        /// </summary>
        /// <returns>The copied card collection.</returns>
        public virtual CardCollection Copy()
        {
            return new CardCollection(this.cards);
        }

        /// <summary>
        /// Determines if a card in the card collection exists with a certain number
        /// </summary>
        /// <param name="number">The number to look for</param>
        /// <returns>Whether or not the card collection has a certain numbered card</returns>
        public bool HasCardWithNumber(CardNumber number)
        {
            bool retval = false;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                if (this[ctr].Number == number)
                {
                    retval = true;
                    break;
                }
            }

            return retval;
        }

        /// <summary>
        /// Determines if a card in the card collection exists with a certain color
        /// </summary>
        /// <param name="color">The color to look for</param>
        /// <returns>Whether or not the card collection has a certain colored card</returns>
        public bool HasCardWithColor(CardColor color)
        {
            bool retval = false;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                if (this[ctr].Color == color)
                {
                    retval = true;
                    break;
                }
            }

            return retval;
        }

        public bool HasCardWithColorAndNotNumber(CardColor color, CardNumber number)
        {
            bool retval = false;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this[ctr];
                if (c.Color == color && c.Number != number)
                {
                    retval = true;
                    break;
                }
            }

            return retval;
        }

        public bool HasCardWithNumberAndNotColor(CardColor color, CardNumber number)
        {
            bool retval = false;

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                Card c = this[ctr];
                if (c.Color != color && c.Number == number)
                {
                    retval = true;
                    break;
                }
            }

            return retval;
        }

        public int DifferentColoredCards()
        {
            int retval = 0;
            bool[] colors = new bool[4];

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                int color = (int)this[ctr].Color;
                if (!colors[color])
                {
                    colors[color] = true;
                    retval++;
                }
            }

            return retval;
        }

        public int DifferentNumberedCards()
        {
            int retval = 0;
            bool[] numbers = new bool[15];

            int count = this.Count;
            for (int ctr = 0; ctr < count; ctr++)
            {
                int number = (int)this[ctr].Number;
                if (number >= 0 && !numbers[number])
                {
                    numbers[number] = true;
                    retval++;
                }
            }

            return retval;
        }

        public int MostContinuousCards(GameRules rules)
        {
            // don't work on this set, work on the copied set, and order it by number
            // so we can work under the assumption that with card [i] and [i+1],
            // num[i] <= num[i+1]
            CardCollection cc = this.Copy();
            cc.OrderByNumber(rules);

            // the last number in the streak
            int lastnum = -1;

            // the most continuous cards yet found
            int max = 1;

            // counter on how many cards are in the current streak
            int ctr = 0;

            for (int ccctr = 0; ccctr < cc.Count; ccctr++)
            {
                Card c = cc[ccctr];

                // only count numbered cards
                if (c.IsNumberedCard())
                {
                    int cNumber = (int)c.Number;
                    // if last num is not set OR this card is of the next number
                    if ((lastnum == -1) || (cNumber == (lastnum + 1)))
                    {
                        // increment our current set
                        ctr++;

                        // if we've surpassed max, set max to our counter
                        if (ctr > max)
                        {
                            max = ctr;
                        }
                    }
                    else if (cNumber != lastnum)
                    {
                        // make sure when we reset the counter, the card number is not
                        // the same as last number (ex: 2,3,4,4,5,6)
                        ctr = 1;
                    }

                    // always set lastnum to c.Number for the next loop
                    lastnum = cNumber;
                }
            }

            return max;
        }

        /// <summary>
        /// Gets the readable name for this Card collection. Returns an empty string
        /// if no cards are in the collection
        /// </summary>
        /// <param name="showid">Whether or not to show the IDs of the cards.</param>
        /// <returns></returns>
        public string ToString(bool showid)
        {
            StringBuilder sb = new StringBuilder("");
            for (int ctr = 0; ctr < this.Count; ctr++)
            {
                Card c = this[ctr];
                if (sb.Length == 0)
                {
                    sb = sb.Append(", ");
                }

                sb = sb.Append(c.ToString());
                
                if (showid)
                {
                    sb = sb.Append(" [");
                    sb = sb.Append(c.Id);
                    sb = sb.Append("]");
                }
            }

            return sb.ToString();
        }
    }
}