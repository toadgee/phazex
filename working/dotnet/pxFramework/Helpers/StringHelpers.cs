namespace PhazeX.Helpers
{
    /// <summary>
    /// This class defines functions that can help when dealing with strings.
    /// </summary>
    public class StringHelpers
    {
        /// <summary>
        /// Replaces in the string the character at position pos with the
        /// character c. Returns the original string if pos is invalid.
        /// </summary>
        /// <param name="s">The string to work on</param>
        /// <param name="c">The character to put in the string</param>
        /// <param name="pos">The position in the string to replace</param>
        /// <returns></returns>
        public static string ReplaceCharacter(string s, char c, int pos)
        {
            if ((pos < 0) || (pos > s.Length))
            {
                return s;
            }

            return s.Substring(0, pos) + c + s.Substring(pos + 1, s.Length - pos - 1);
        }

        /// <summary>
        /// Removes all non-numeric characters in a string. Returns the
        /// totally numeric string.
        /// </summary>
        /// <param name="s">The string to work on.</param>
        /// <returns>The totally numeric string.</returns>
        public static string RemoveAllNonNumeric(string s)
        {
            string retVal = "";
            for (int ctr = 0; ctr < s.Length; ctr++)
            {
                char c = s[ctr];

                if (char.IsNumber(c))
                {
                    retVal += c;
                }
            }

            return retVal;
        }
    }
}