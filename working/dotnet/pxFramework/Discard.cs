namespace PhazeX
{
    using System.Collections.Generic;
    using PhazeX.Helpers;

    /// <summary>
    /// This class simulates a discard pile. Operates like a stack, except that we can only remove the top card 
    /// once. Must replace before we pop something off the top again.
    /// </summary>
    public class Discard
    {
        private List<Card> discard = new List<Card>();

        /// <summary>
        /// Create a new discard pile.
        /// </summary>
        public Discard()
        {
        }

        /// <summary>
        /// Removes and returns the top card in the discard pile.
        /// </summary>
        /// <returns>The top card.</returns>
        public Card RemoveTopCard()
        {
            if (this.discard.Count == 0)
            {
                return null;
            }

            Card c = this.discard[0];
            this.discard.RemoveAt(0);
            return c;
        }

        /// <summary>
        /// Returns without removing the top card in the discard pile.
        /// </summary>
        /// <returns>The top card.</returns>
        public Card ReadTopCard()
        {
            if (this.discard.Count == 0)
            {
                return null;
            }

            return this.discard[0];
        }

        /// <summary>
        /// Adds the card to the top of the discard pile
        /// </summary>
        /// <param name="card">Card to add to the top of the discard pile</param>
        public void AddTopCard(Card card)
        {
            ExceptionHelpers.CheckNotNull(card, "card");
            this.discard.Insert(0, card);
        }

        /// <summary>
        /// Removes all cards below the top card. Useful for when the deck
        /// gets empty and we want to shuffle them back into the deck.
        /// </summary>
        /// <returns>A linked list of the cards that are unused in the discard pile. Can be null.</returns>
        public List<Card> RemoveAllButTopCard()
        {
            Card c = this.RemoveTopCard();
            List<Card> retval = this.discard;
            this.discard = new List<Card>();
            this.discard.Add(c);
            return retval;
        }
    }
}