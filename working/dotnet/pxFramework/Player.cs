namespace PhazeX
{
    using PhazeX.Options;
    using PhazeX.Helpers;
    using System;

    public class Player
    {
        /// <summary>
        /// Creates a player object
        /// </summary>
        public Player()
            : this(false)
        {
        }

        public Player(string name)
            : this(false)
        {
            this.Name = name;
        }

        public Player(bool computerPlayer, string name)
            : this(computerPlayer)
        {
            this.Name = name;
        }

        public Player(bool computerPlayer)
        {
            this.Connected = true;
            this.LoggedIn = false;
            this.ComputerPlayer = computerPlayer;
            this.Scoreboard = new Scoreboard();
            this.SkipsLeft = 0;
            this.CompletedPhaze = false;
            this.Name = "Unknown :: Not logged in!";
            this.PlayerID = -1;
            this.MyTurn = false;
            this.ChangedName = false;
            this.ReadyToPlay = false;
            this.PickedUpCard = false;
        }

        /// <summary>
        /// Player's name
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Player's ID
        /// </summary>
        public int PlayerID
        {
            get;
            set;
        }

        /// <summary>
        /// Player's client program release
        /// </summary>
        public int Release
        {
            get;
            set;
        }

        /// <summary>
        /// Player's client program revision
        /// </summary>
        public int Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Player's client program build
        /// </summary>
        public int Build
        {
            get;
            set;
        }

        /// <summary>
        /// Player's client framework release
        /// </summary>
        public int LibRelease
        {
            get;
            set;
        }

        /// <summary>
        /// Player's client framework revision
        /// </summary>
        public int LibRevision
        {
            get;
            set;
        }

        /// <summary>
        /// Player's client framework build
        /// </summary>
        public int LibBuild
        {
            get;
            set;
        }

        /// <summary>
        /// Player's client title
        /// </summary>
        public string ClientTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Player's client framework title
        /// </summary>
        public string FrameworkVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Player's operating system version
        /// </summary>
        public string OSVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Whether or not it's the player's turn
        /// </summary>
        public bool MyTurn
        {
            get;
            set;
        }

        /// <summary>
        /// Whether or not they've changed their name - only allows for it once
        /// </summary>
        public bool ChangedName
        {
            get;
            set;
        }

        /// <summary>
        /// Whether or not they've completed their phaze
        /// </summary>
        public bool CompletedPhaze
        {
            get;
            set;
        }

        /// <summary>
        /// Whether or not they're ready to play
        /// </summary>
        public bool ReadyToPlay
        {
            get;
            set;
        }

        /// <summary>
        /// Whether or not they've picked up their card
        /// </summary>
        public bool PickedUpCard
        {
            get;
            set;
        }

        /// <summary>
        /// Their current phaze
        /// </summary>
        public int CurrentPhaze
        {
            get;
            set;
        }

        public PhazeRule GetCurrentPhazeRule(GameRules rules)
        {
            return rules.PhazeRules.Phaze(this.CurrentPhaze);
        }

        public bool ComputerPlayer
        {
            get;
            set;
        }

        /// <summary>
        /// Number of skips pending. Gets reset at the beginning of every hand.
        /// </summary>
        public int SkipsLeft
        {
            get;
            set;
        }

        /// <summary>
        /// The scoreboard for the player for the entire game.
        /// </summary>
        public Scoreboard Scoreboard
        {
            get;
            set;
        }

        /// <summary>
        /// Whether or not the player is connected
        /// </summary>
        public bool Connected
        {
            get;
            set;
        }

        public Hand Hand
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether or not this player has any skips pending.
        /// </summary>
        /// <returns>Whether or not they have skips pending</returns>
        public bool IsSkipped
        {
            get
            {
                return (this.SkipsLeft > 0);
            }
        }

        /// <summary>
        /// Whether or not the player is logged int
        /// </summary>
        protected bool LoggedIn
        {
            get;
            set;
        }

        /// <summary>
        /// This player skipped player p. Update the scoreboard
        /// </summary>
        /// <param name="player">Player who this player skipped</param>
        public void Skip(Player player)
        {
            ExceptionHelpers.CheckNotNull(player, "player");
            
            this.Scoreboard.CurrentSession.AddPlayerSkipped(player.PlayerID);
        }

        /// <summary>
        /// Skips this player. Skip came from player p.
        /// </summary>
        /// <param name="player">Player who skipped this player</param>
        public void SkippedBy(Player player)
        {
            ExceptionHelpers.CheckNotNull(player, "player");
            
            // get current session
            this.Scoreboard.CurrentSession.AddSkippedBy(player.PlayerID);
            this.SkipsLeft++;    
        }

        /// <summary>
        /// Removes one skip pending.
        /// </summary>
        public void Skipped()
        {
            if (this.SkipsLeft > 0)
            {
                this.SkipsLeft--;
            }
        }

        /// <summary>
        /// The player must pick a card.
        /// </summary>
        public virtual void PickCard()
        {
            throw new NotImplementedException("Player::PickCard is not implemented by the deriving class!");
        }

        /// <summary>
        /// The player must play.
        /// </summary>
        public virtual void Play()
        {
            throw new NotImplementedException("Player::Play is not implemented by the deriving class!");
        }

        /// <summary>
        /// The player must discard.
        /// </summary>
        public virtual void Discard()
        {
            throw new NotImplementedException("Player::Discard is not implemented by the deriving class!");
        }

        /// <summary>
        /// Returns the player's name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}