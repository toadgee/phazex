namespace PhazeX.Helpers
{
    using System;
    
    /// <summary>
    /// This class defines all definitions for cards as well as the names for them.
    /// </summary>
    public static class CardInfo
    {
        /// <summary>
        /// Returns the complete name of a card (with or without spacing). Will throw an exception if c is null or if the card can't be decoded.
        /// </summary>
        /// <param name="c">Card to title.</param>
        /// <param name="colorSpaced">If true, it will space the color so that no matter which color it is, it will be the same length</param>
        /// <param name="numberSpaced">If true, it will space the number so that no matter which number it is, it will be the same length</param>
        /// <returns>The complete name of the card</returns>
        public static string GetName(Card c, bool colorSpaced, bool numberSpaced)
        {
            if (c == null)
            {
                throw new Exception(GameLibraryVersion.ProgramIdentifier + " : Card info got null card");
            }

            string color = GetColorName(c);
            if (colorSpaced)
            {
                color = color.PadRight(7, ' ');
            }

            string number = GetNumberName(c);
            if (numberSpaced && c.IsNumberedCard())
            {
                number = number.PadLeft(2, '0');
            }

            return color + " " + number;
        }

        /// <summary>
        /// Returns the name of the card without spacing. The equivalent of GetName(c, false, false). Will throw an exception if c is null or if the card can't be decoded.
        /// </summary>
        /// <param name="c">Card to title.</param>
        /// <returns>The complete name of the card.</returns>
        public static string GetName(Card c)
        {
            if (c == null)
            {
                throw new Exception(GameLibraryVersion.ProgramIdentifier + " : Card info got null card");
            }

            string color = GetColorName(c);
            string number = GetNumberName(c);
            return color + " " + number;
        }

        /// <summary>
        /// Returns the name of a color of a card. Will throw an exception if c is null or if the color is unknown.
        /// </summary>
        /// <param name="c">Card to title the color of</param>
        /// <returns>The name of the color of the card.</returns>
        public static string GetColorName(Card c)
        {
            if (c == null)
            {
                throw new Exception(GameLibraryVersion.ProgramIdentifier + " : Card info got null card");
            }

            string color = "unknown";

            if (c.Color == CardColor.Red)
            {
                color = "Red";
            }
            else if (c.Color == CardColor.Yellow)
            {
                color = "Yellow";
            }
            else if (c.Color == CardColor.Green)
            {
                color = "Green";
            }
            else if (c.Color == CardColor.Blue)
            {
                color = "Blue";
            }
            else
            {
                throw new Exception(GameLibraryVersion.ProgramIdentifier + " : Card info can't tell what color this card is (" + c.Color + ").");
            }

            return color;
        }

        /// <summary>
        /// Returns the name of the number of a card. Equivalent of calling GetNumberName(c, true)
        /// </summary>
        /// <param name="c">Card to determine the readable number of.</param>
        /// <returns>The readable number</returns>
        public static string GetNumberName(Card c)
        {
            return GetNumberName(c, true);
        }

        /// <summary>
        /// Returns the name of the number of a card. If showWild is true, it will display wild(#). If false, it will display the number. May thrown an exception if c is null or the number cannot be decoded.
        /// </summary>
        /// <param name="c">Card to determine the readable number of.</param>
        /// <param name="showWild">If true, displays as a wild with the assigned number (if applicable in parentheses). If false displays just the number (if applicable).</param>
        /// <returns>The readable number of the card.</returns>
        public static string GetNumberName(Card c, bool showWild)
        {
            if (c == null)
            {
                throw new Exception("Card info got null card");
            }
            
            string number = "unknown";

            if (c.IsNumberedCard() && (!c.Wild))
            {
                number = ((int)c.Number).ToString();
            }
            else if (c.Number == CardNumber.Wild || c.Wild)
            {
                if (showWild)
                {
                    number = "Wild";
                    if (c.Wild && (c.Number != CardNumber.Wild))
                    {
                        number += " (" + ((int)c.Number).ToString() + ")";
                    }
                }
                else
                {
                    if (c.Wild && (c.Number == CardNumber.Wild))
                    {
                        number = "Wild";
                    }
                    else
                    {
                        return GetNumberName(new Card(c.Number, c.Color, c.Id));
                    }
                }
            }
            else if (c.Number == CardNumber.Skip)
            {
                number = "Skip";
            }
            else if (c.Number == CardNumber.Reverse)
            {
                number = "Reverse";
            }
            else if (c.Number == CardNumber.Draw)
            {
                number = "Draw";
            }
            else
            {
                throw new Exception(GameLibraryVersion.ProgramIdentifier + " : Card info can't tell what number this card is (" + c.Number + ").");
            }

            return number;
        }
    }
}
