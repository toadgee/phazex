namespace PhazeX.Helpers
{
    using System;
    using System.Diagnostics;

    public static class ExceptionHelpers
    {
        [Conditional("DEBUG")]
        public static void CheckNotNull(object instance, string name)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        [Conditional("DEBUG")]
        public static void CheckCurrentPlayer(Player player, Player currentPlayer)
        {
            if (player != currentPlayer)
            {
                throw new InvalidOperationException("It is not the player's turn!");
            }
        }
    }
}
