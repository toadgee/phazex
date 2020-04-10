namespace PhazeX
{
    using System;
    using System.Collections.Generic;
    using PhazeX.Options;
    using PhazeX.Helpers;

    /// <summary>
    /// This class represents a deck.
    /// </summary>
    public class Deck
    {
        private readonly Random rand = new Random();
        private readonly List<Card> deck = new List<Card>();

        /// <summary>
        /// Create a new deck based on the game rules. First creates all of
        /// the cards, and then shuffles into a new pile. This new pile is the
        /// one assigned to the deck.
        /// </summary>
        /// <param name="rules">Game rules for the game.</param>
        public Deck(GameRules rules)
        {
            Card c = null;

            // create the deck in order first
            int id = 0;
            for (CardNumber num = rules.MinimumCardNumber; num <= rules.MaximumCardNumber; num++)
            {
                for (CardColor col = rules.MinimumCardColor; col <= rules.MaximumCardColor; col++)
                {
                    for (int ctr = 0; ctr < rules.CardsCount(num, col); ctr++)
                    {
                        this.deck.Add(new Card(num, col, id));
                        id++;
                    }
                }
            }

            // and then shuffle
            List<Card> new_deck = new List<Card>();
            while (this.deck.Count > 0)
            {
                c = this.deck[this.rand.Next(this.deck.Count)];
                this.deck.Remove(c);
                new_deck.Add(c);
            }

            this.deck.AddRange(new_deck);
        }

        /// <summary>
        /// Remove the top card from the deck.
        /// </summary>
        /// <returns>The top card</returns>
        public Card RemoveCard()
        {
            if (this.deck.Count == 0)
            {
                throw new InvalidOperationException("Deck is empty!");
            }

            Card c = this.deck[0];
            this.deck.RemoveAt(0);
            return c;
        }

        /// <summary>
        /// Note: This function currently is stubbed out.
        /// This function is passed a linked list of cards, shuffles them
        /// randomly, and adds them back to the bottom of the deck. This
        /// should be called by the server whenever we run out of cards.
        /// It will take them from the discard pile (leaving the top one or
        /// two) and shuffle them back into the deck.
        /// </summary>
        /// <param name="cards">Linked list of cards to shuffle back into the deck</param>
        public void ShuffleAndAddUnused(List<Card> cards)
        {
            ExceptionHelpers.CheckNotNull(cards, "cards");

            // this is used when we run out of cards
            // or a player disconnects
            // or something
            for (int ctr = 0; ctr < cards.Count; ctr++)
            {
                // determine where to add cards in deck
                this.deck.Insert(
                    this.rand.Next(this.deck.Count),
                    cards[ctr]);
            }

            return;
        }

        /// <summary>
        /// Determines if the top card is null or not (meaning the deck is empty).
        /// </summary>
        /// <returns>True if no cards exist in the deck.</returns>
        public bool IsTopCardNull()
        {
            return (this.deck.Count == 0);
        }
    }
}