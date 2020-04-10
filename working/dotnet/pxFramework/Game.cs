namespace PhazeX
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using PhazeX.Automation;
    using PhazeX.Helpers;
    using PhazeX.Options;
    using System.Diagnostics;

    public class Game
    {
        private readonly object monitor = new object();

        private GameRules rules = new GameRules();
        private List<Player> players = new List<Player>();
        private Deck deck;
        private Discard discard;
        private Player currentPlayer;
        private Player dealerPlayer;
        private int unusedPlayerID;

        /// <summary>
        /// Used for counting the turn in the current hand
        /// </summary>
        private int turn;

        public Game()
        {
            this.State = GameState.GameInitialized;
        }

        public Game(Player player, int computerPlayers)
            : this(computerPlayers)
        {
            ExceptionHelpers.CheckNotNull(player, "player");
            this.AddPlayer(player);
        }

        public Game(int computerPlayers)
            : this()
        {
            if (computerPlayers > this.Rules.MaximumPlayers)
            {
                throw new ArgumentException("Must supply between 0 and Maximum Players", "computerPlayers");
            }

            for (int ctr = 0; ctr < computerPlayers; ctr++)
            {
                new AutomatedPlayer(this);
            }
        }

        public event EventHandler<GameStartEventArgs> GameStart;
        public event EventHandler<HandStartEventArgs> HandStart;
        public event EventHandler<TurnStartEventArgs> TurnStart;
        public event EventHandler<TookDeckCardEventArgs> TookDeckCard;
        public event EventHandler<TookDiscardEventArgs> TookDiscard;
        public event EventHandler<MakePhazeEventArgs> MakePhaze;
        public event EventHandler<PlayOnTableEventArgs> PlayOnTable;
        public event EventHandler<PutDiscardEventArgs> PutDiscard;
        public event EventHandler<SkippedEventArgs> Skipped;
        public event EventHandler<SkipEventArgs> Skip;
        public event EventHandler<TurnEndEventArgs> TurnEnd;
        public event EventHandler<HandEndEventArgs> HandEnd;
        public event EventHandler<GameEndEventArgs> GameEnd;
        public event EventHandler<GameKilledEventArgs> GameKilled;

        /// <summary>
        /// Total turns per game.
        /// </summary>
        public int TotalTurns
        {
            get;
            private set;
        }

        public GameRules Rules
        {
            get
            {
                return this.rules;
            }
        }

        public GameState State
        {
            get;
            private set;
        }

        public Table Table
        {
            get;
            private set;
        }

        public Card TopDiscard
        {
            get
            {
                return this.discard.ReadTopCard().CarbonCopy();
            }
        }

        /// <summary>
        /// Hand number for the game.
        /// </summary>
        public int HandNumber
        {
            get;
            private set;
        }

        public Player[] PlayerList
        {
            get
            {
                Player[] players = new Player[this.players.Count];
                for (int ctr = 0; ctr < this.players.Count; ctr++)
                {
                    players[ctr] = this.players[ctr];
                }

                return players;
            }
        }

        public void AddPlayer(Player player)
        {
            ExceptionHelpers.CheckNotNull(player, "player");
            
            player.PlayerID = this.unusedPlayerID;
            this.unusedPlayerID++;
            this.players.Add(player);

            if (this.players.Count == this.Rules.MaximumPlayers)
            {
                PhazeXLog.LogInformation("Reached player limit, starting game.");
                this.StartGame();
            }
        }

        public Player WhoPlaysNext(Player source)
        {
            ExceptionHelpers.CheckNotNull(source, "source");

            int sourcePos = -1;
            for (int ctr = 0; ctr < this.players.Count; ctr++)
            {
                if (this.players[ctr] == source)
                {
                    sourcePos = ctr;
                    break;
                }
            }

            if (sourcePos == -1)
            {
                throw new ArgumentException("Couldn't find source player in player list.");
            }

            for (int ctr = sourcePos + 1; ctr != sourcePos; ctr++)
            {
                if (ctr == this.players.Count)
                {
                    ctr = 0;
                }

                if (!this.players[ctr].IsSkipped)
                {
                    return this.players[ctr];
                }
            }

            return source;
        }

        public void ReadyToPlay(Player source)
        {
            ExceptionHelpers.CheckNotNull(source, "source");

            source.ReadyToPlay = true;

            // check to see if everybody is ready!
            int humanPlayers = 0;
            int humanPlayersReady = 0;
            
            for (int ctr = 0; ctr < this.players.Count; ctr++)
            {
                Player player = this.players[ctr];
                
                if (player.ComputerPlayer)
                {
                    continue;
                }

                humanPlayers++;

                if (player.ReadyToPlay)
                {
                    humanPlayersReady++;
                }
            }

            if (humanPlayers == humanPlayersReady && humanPlayers != 0)
            {
                this.StartGame();
            }
        }
        
        public void StartGameInBackground()
        {
            Thread t = new Thread(new ThreadStart(this.StartGame));
            t.IsBackground = true;
            t.Name = "Game Thread";
            t.Start();
        }
        
        public void StartGame()
        {
            Debug.WriteLine("Starting game...");
            if (this.players.Count < 2 || this.players.Count > this.Rules.MaximumPlayers)
            {
                throw new InvalidOperationException("Cannot start game with this number of players");
            }

            for (int ctr = 0; ctr < this.players.Count; ctr++)
            {
                this.players[ctr].Scoreboard = new Scoreboard();
            }

            while (this.State != GameState.EndOfGame)
            {
                Debug.WriteLine(this.State);
                switch (this.State)
                {
                    
                }
            }
        }

        public void GetDeckCard(Player source)
        {
            ExceptionHelpers.CheckNotNull(source, "source");

            source.Hand.Add(this.deck.RemoveCard());

            if (this.deck.IsTopCardNull())
            {
                this.deck.ShuffleAndAddUnused(this.discard.RemoveAllButTopCard());
            }

            this.OnTookDeckCard(source, this.discard.ReadTopCard().CarbonCopy());
        }

        public void GetDiscard(Player source)
        {
            ExceptionHelpers.CheckNotNull(source, "source");

            Card c = this.discard.RemoveTopCard();
            source.Hand.Add(c.CarbonCopy());

            this.OnTookDiscard(source, c.CarbonCopy());
        }

        public Card ReadDiscard()
        {
            return this.discard.ReadTopCard().CarbonCopy();
        }


        public void Discard(Card card, Player source)
        {
            Debug.WriteLine("Discarding...");
            ExceptionHelpers.CheckNotNull(card, "card");
            ExceptionHelpers.CheckNotNull(source, "source");

            this.discard.AddTopCard(card);

            this.OnPutDiscard(source, card);
        }

        public void Discard(Card card, Player skipped, Player source)
        {
            Debug.WriteLine("Discarding skip...");
            ExceptionHelpers.CheckNotNull(card, "card");
            ExceptionHelpers.CheckNotNull(skipped, "skipped");
            ExceptionHelpers.CheckNotNull(source, "source");
            ExceptionHelpers.CheckCurrentPlayer(source, this.currentPlayer);

            if (card.Number != CardNumber.Skip)
            {
                throw new ArgumentException("Card is not a skip card!");
            }

            this.discard.AddTopCard(card);
            skipped.SkippedBy(source);

            this.OnPutDiscard(source, card);
            this.OnSkipped(skipped, source);
        }

        public bool EndHand()
        {
            Debug.WriteLine("End hand...");
            // determine if hand anybody has won the game
            bool endGame = false;

            for (int players_ctr = 0; players_ctr < this.players.Count; players_ctr++)
            {
                Player p = this.players[players_ctr];
                
                // calculate session details
                Session s = p.Scoreboard.CurrentSession;
                s.MadePhaze = p.CompletedPhaze;
                if (p.CompletedPhaze)
                {
                    p.CurrentPhaze++;
                    if (p.CurrentPhaze == this.Rules.PhazeRules.Count())
                    {
                        endGame = true;
                    }
                }

                // card points
                for (int ctr = 0; ctr < p.Hand.Count; ctr++)
                {
                    s.Points += this.Rules.GetCardPointValue(p.Hand[ctr].Number);
                }

                // now, reset attributes
                p.CompletedPhaze = false;

                // remove pending skips!
                while (p.SkipsLeft > 0)
                {
                    p.Skipped();
                }

                p.MyTurn = false;
                p.PickedUpCard = false;
            }

            this.OnHandEnd(null);

            return endGame;
        }

        public void EndGame()
        {
            Debug.WriteLine("Game end...");
            // determine winner
            int bestPhazeNumber = 0;
            int bestPoints = 0;
            List<Player> winners = new List<Player>(this.players.Count);
            for (int ctr = 0; ctr < this.players.Count; ctr++)
            {
                Player p = this.players[ctr];
                int phaze = p.Scoreboard.CompletedPhazeNumber;
                int points = p.Scoreboard.Points;

                // if phaze is better than the best phaze, it's our new winner!
                // if player points is less than best points, it's our new winner!
                if ((phaze > bestPhazeNumber) || (phaze == bestPhazeNumber && points <= bestPoints))
                {
                    // check for tie
                    if (points == bestPoints)
                    {
                        winners.Add(p);
                    }
                    else
                    {
                        winners.Clear();
                        winners.Add(p);
                        bestPhazeNumber = phaze;
                        bestPoints = points;
                    }
                }
            }

            this.State = GameState.EndOfGame;
            this.OnGameEnd(winners);
        }

        public void KillGame()
        {
            Debug.WriteLine("Killing game...");
            this.State = GameState.EndOfGame;
            this.OnGameKilled();
        }

        private void StartHand()
        {
            Debug.WriteLine("Starting hand...");
            this.HandNumber++;
            this.turn = 0;
            this.deck = new Deck(this.Rules);
            this.discard = new Discard();
            this.Table = new Table();

            // figure out dealer player & first player
            this.dealerPlayer = this.players[this.HandNumber % this.players.Count];
            this.currentPlayer = this.WhoPlaysNext(this.dealerPlayer);

            // deal
            for (int ctr = 0; ctr < this.players.Count; ctr++)
            {
                Player p = this.players[ctr];
                p.Hand = new Hand();
                p.Scoreboard.AddSession(new Session(this.dealerPlayer == p, p.CurrentPhaze));
            }

            for (int ctr = 0; ctr < this.Rules.HandCards; ctr++)
            {
                for (int ctr2 = 0; ctr2 < this.players.Count; ctr2++)
                {
                    this.players[ctr2].Hand.Add(this.deck.RemoveCard());
                }
            }

            this.discard.AddTopCard(this.deck.RemoveCard());
            this.OnHandStart(this.dealerPlayer);
        }

        private bool EndTurn()
        {
            Debug.WriteLine("Ending turn...");
            this.currentPlayer.MyTurn = false;
            this.currentPlayer.PickedUpCard = false;

            bool retval = false;
            if (this.currentPlayer.Hand.Count == 0)
            {
                retval = true;
            }
            else
            {
                // find new player turn!
                int currentPlayerPos = this.HandNumber % this.players.Count + this.turn;
                this.currentPlayer = null;
                for (int ctr = currentPlayerPos + 1; ctr != currentPlayerPos; ctr++)
                {
                    if (this.players.Count <= ctr)
                    {
                        ctr %= this.players.Count;
                    }

                    if (this.players[ctr].IsSkipped)
                    {
                        this.players[ctr].Skipped();
                        this.OnSkip(this.players[ctr]);
                        continue;
                    }

                    this.currentPlayer = this.players[ctr];
                    break;
                }

                if (this.currentPlayer == null)
                {
                    this.currentPlayer = this.players[currentPlayerPos % this.players.Count];
                }

                // increment turn!
                this.turn++;
                this.TotalTurns++;
                this.currentPlayer.MyTurn = true;
                this.currentPlayer.PickedUpCard = false;
            }

            return retval;
        }        

        private void OnGameKilled()
        {
            EventHandler<GameKilledEventArgs> eh = this.GameKilled;
            if (eh != null)
            {
                eh(this, new GameKilledEventArgs());
            }
        }

        private void OnGameEnd(List<Player> winners)
        {
            EventHandler<GameEndEventArgs> eh = this.GameEnd;
            if (eh != null)
            {
                eh(this, new GameEndEventArgs(winners));
            }
        }

        private void OnGameStart(List<Player> players)
        {
            EventHandler<GameStartEventArgs> eh = this.GameStart;
            if (eh != null)
            {
                eh(this, new GameStartEventArgs(players));
            }
        }

        private void OnHandStart(Player dealerPlayer)
        {
            EventHandler<HandStartEventArgs> eh = this.HandStart;
            if (eh != null)
            {
                eh(this, new HandStartEventArgs(dealerPlayer));
            }
        }

        private void OnHandEnd(Player player)
        {
            EventHandler<HandEndEventArgs> eh = this.HandEnd;
            if (eh != null)
            {
                eh(this, new HandEndEventArgs(player));
            }
        }

        private void OnStartTurn(Player currentPlayer)
        {
            Debug.WriteLine("Starting turn for " + currentPlayer.Name + "...");
            EventHandler<TurnStartEventArgs> eh = this.TurnStart;
            if (eh != null)
            {
                eh(this, new TurnStartEventArgs(this.currentPlayer));
            }
        }

        private void OnTookDeckCard(Player player, Card discard)
        {
            EventHandler<TookDeckCardEventArgs> eh = this.TookDeckCard;
            if (eh != null)
            {
                eh(this, new TookDeckCardEventArgs(player, discard));
            }
        }

        private void OnTookDiscard(Player player, Card discard)
        {
            EventHandler<TookDiscardEventArgs> eh = this.TookDiscard;
            if (eh != null)
            {
                eh(this, new TookDiscardEventArgs(player, discard));
            }
        }

        private void OnSkip(Player player)
        {
            EventHandler<SkipEventArgs> eh = this.Skip;
            if (eh != null)
            {
                eh(this, new SkipEventArgs(player));
            }
        }

        private void OnTurnEnd(Player player)
        {
            EventHandler<TurnEndEventArgs> eh = this.TurnEnd;
            if (eh != null)
            {
                eh(this, new TurnEndEventArgs(player));
            }
        }

        private void OnMakePhaze(Player player, List<Group> groups)
        {
            EventHandler<MakePhazeEventArgs> eh = this.MakePhaze;
            if (eh != null)
            {
                eh(this, new MakePhazeEventArgs(player, groups));
            }
        }

        private void OnPlayOnTable(Player player, Card card, Group group)
        {
            EventHandler<PlayOnTableEventArgs> eh = this.PlayOnTable;
            if (eh != null)
            {
                eh(this, new PlayOnTableEventArgs(player, card, group));
            }
        }

        private void OnPutDiscard(Player player, Card card)
        {
            EventHandler<PutDiscardEventArgs> eh = this.PutDiscard;
            if (eh != null)
            {
                eh(this, new PutDiscardEventArgs(player, card));
            }
        }

        private void OnSkipped(Player skipped, Player skipper)
        {
            EventHandler<SkippedEventArgs> eh = this.Skipped;
            if (eh != null)
            {
                eh(this, new SkippedEventArgs(skipped, skipper));
            }
        }
    }
}
