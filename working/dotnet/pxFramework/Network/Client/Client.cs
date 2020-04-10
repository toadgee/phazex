using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using PhazeX.Helpers;
using PhazeX.Network.Messages;
using PhazeX.Network.Server;
using PhazeX.Options;

namespace PhazeX.Network.Client
{
	public class Client
	{
        private Buffer buffer = new Buffer();
        private readonly object socketSync = new object();

        private NetworkSettings networkSettings;
        private PlayerSettings playerSettings;

        public GameRules GameRules
        {
            get;
            private set;
        }

        #region class variables
        protected Player [] _players = null;
		protected Socket _sock = null;
		protected byte [] _messageBuffer = new byte[0];
		public MessageProcessor _mp = new MessageProcessor();

        protected bool _computerPlayer = false;

		private ServerGame _serverGame = null;	//will be null unless _hosting is true
		public ServerGame _ServerGame
		{
			get { return _serverGame; }
		}

		private bool _hosting = false;
		public bool _Hosting
		{
			get { return _hosting; }
		}

		private Hand _hand = null;
		public Hand _Hand
		{
			get { return _hand; }
			set { lock(this) { _hand = value; } }
		}
		private Card _discard = null;
		public Card _Discard
		{
			get { return _discard; }
			set { lock(this) { _discard = value; } }
		}
		private Table _table = null;
		public Table _Table
		{
			get { return _table; }
			set { lock(this) { _table = value; } }
		}
		private int _firstDiscard = 2;
		public int _FirstDiscard
		{
			get { return _firstDiscard; }
			set { lock(this) { _firstDiscard = value; } }
		}

		private bool _myTurn = false;
		public bool _MyTurn
		{
			get { return _myTurn; }
			set { lock(this) {_myTurn = value; } }
		}
		//private bool _madePhaze = false;	//have we made our phaze?
		public bool _MadePhaze
		{
			get { return GetPlayerWithID(_PlayerID).CompletedPhaze; }
            set { lock (this) { GetPlayerWithID(_PlayerID).CompletedPhaze = value; } }
		}
		private bool _pickedCard = false;	//have we picked a card this hand?
		public bool _PickedCard
		{
			get { return _pickedCard; }
			set { lock(this) { _pickedCard = value; } }
		}
		private bool _connected = false;		
		public bool _Connected
		{
			get { return _connected; }
		}

		private bool _loggedIn = false;
		public bool _LoggedIn
		{
			get { return _loggedIn; }
			set { lock(this) { _loggedIn = value; } }
		}
		private int _playerID = -1;
		public int _PlayerID
		{
			get { return _playerID; }
		}

		private bool _readyToPlay = false;
		public bool _ReadyToPlay
		{
			get { return _readyToPlay; }
			set { readyToPlay(value); }
		}
		private bool _gameStarted = false;
		public bool _GameStarted
		{
			get { return _gameStarted; }
			set { _gameStarted = value; }
		}

		private int _currentPhazeRuleNumber;
		public int _CurrentPhazeRuleNumber
		{
			get { return _currentPhazeRuleNumber; }
			set { _currentPhazeRuleNumber = value; }
		}
		

		#endregion

		public Client(NetworkSettings networkSettings, PlayerSettings playerSettings, GameRules gameRules)
		{
            ExceptionHelpers.CheckNotNull(networkSettings, "networkSettings");
            ExceptionHelpers.CheckNotNull(playerSettings, "playerSettings");
            ExceptionHelpers.CheckNotNull(gameRules, "gameRules");

            this.networkSettings = networkSettings;
            this.playerSettings = playerSettings;
            this.GameRules = gameRules;
		}

        public int BytesSent
        {
            get;
            private set;
        }

        public int BytesReceived
        {
            get;
            private set;
        }

		public bool Host()
		{
			_serverGame = new ServerGame(this.GameRules, this.networkSettings);
			this.networkSettings._Hostname = "127.0.0.1";
			if (_serverGame.Started())
			{
				_hosting = true;
			}
			else
			{
				_hosting = false;
			}
			return _hosting;
		}
		#region connect/disconnect
		public virtual void Connect()
		{
            
			//connect
			try
			{
                
                IPHostEntry iphe;

                string address = this.networkSettings._Hostname;

                if (address.Equals("127.0.0.1"))
                {
                    try
                    {
                        IPEndPoint ipe = new IPEndPoint(IPAddress.Loopback, this.networkSettings._Port);
                        Socket s = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        s.NoDelay = true;
                        s.Connect(ipe);
                        _sock = s;


                        //set up heartbeat!
                        Thread t = new Thread(new ThreadStart(this.DoHeartbeat));
                        t.Name = "Heartbeat Thread to server";
                        t.Start();
                    }
                    catch (Exception) {  }
                }
                else
                {
                    iphe = Dns.GetHostEntry(this.networkSettings._Hostname);
                    
                    foreach (IPAddress ipa in iphe.AddressList)
                    {
                        //bug fix: on ipv6, sometimes the timeouts are too long
                        if (ipa.AddressFamily == AddressFamily.InterNetworkV6) continue;
                        try
                        {
                            IPEndPoint ipe = new IPEndPoint(ipa, this.networkSettings._Port);
                            Socket s = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                            s.NoDelay = true;
                            s.Connect(ipe);
                            _sock = s;


                            //set up heartbeat!
                            Thread t = new Thread(new ThreadStart(this.DoHeartbeat));
                            t.Name = "Heartbeat Thread to server";
                            t.Start();
                            break;
                        }
                        catch (Exception) { continue; }
                    }
                }
			}
			catch (Exception e)
			{
				PhazeXLog.LogError(e, GameLibraryVersion.ProgramIdentifier, 100);
				_sock = null;
			}
			_connected = (_sock != null);
            
			return;
		}
        public void DoHeartbeat()
        {
            while (_connected)
            {
                if (!Send(new HeartBeatMessage())) break;
                Thread.Sleep(TimingDefinitions.Heartbeat_Client);
            }
        }

		public void CheckConnectivity()
		{
			if (_connected)
			{
				Send(new HeartBeatMessage());
			}
		}
		public void Disconnect()
		{
			try
			{
				lock(this.socketSync)
				{
					_connected = false;
                    if (_sock != null)
                    {
                        _sock.Close();
                    }
				}
			}
			catch (Exception ex)
			{
				PhazeXLog.LogError(ex, GameLibraryVersion.VersionString, 101);				
			}

			try
			{
				if (_serverGame != null)
				{
					_serverGame.Cancel();
				}
			}
			catch (Exception ex)
			{
				PhazeXLog.LogError(ex, GameLibraryVersion.VersionString, 102);				
			}
		}
		public virtual bool Send(Message message)
		{
            if (message == null) return true;
            byte[] message_text = message.MessageText;
            message_text = Message.Encode(message_text);
			bool val = true;
			try
			{
                lock (this.socketSync)
				{
					int sent = _sock.Send(message_text);
					if ((message_text.Length * 2) != sent)
					{
						val = false;
					}
                    
                    this.BytesSent += sent;
				}
			}
			catch (Exception ex)
			{
				PhazeXLog.LogError(ex, GameLibraryVersion.VersionString, 103);
				Disconnect();
				return false;
			}
			return val;
		}
		protected Message getMessage()
		{
            byte[] b;
            int avail;
            try
            {
                //while we're here, look for data in the buffer
                avail = _sock.Available;
                if (avail > 0)
                {
                    buffer.ReadFromSocket(_sock, avail);
                    
                    this.BytesReceived += avail;
                }
                
				//first see if we can actually decode something
                if (buffer.CanDecode())
                {
                    b = buffer.Decode();
                }
                else return null;
                
			}
			catch (Exception ex)
			{
				PhazeXLog.LogError(ex, GameLibraryVersion.VersionString, 104);
				_connected = false;
				return null;
			}




            Message m = null;
            if (b.Length == 0) return m;
            else if (b[0] == (byte)pxMessages.Heartbeat) m = new HeartBeatMessage(b);
            else if (b[0] == (byte)pxMessages.ChangeName) m = new ChangeNameMessage(b, this.GetPlayerIDs());
            else if (b[0] == (byte)pxMessages.ChangeNameReject) m = new ChangeNameRejectMessage(b);
            else if (b[0] == (byte)pxMessages.Chat) m = new ChatMessage(b, this.GetPlayerIDs());
            else if (b[0] == (byte)pxMessages.CompletedPhaze) m = new CompletedPhazeMessage(b, this.GetPlayerIDs());
            else if (b[0] == (byte)pxMessages.CurrentPhaze) m = new CurrentPhazeMessage(b, this.GameRules);
            else if (b[0] == (byte)pxMessages.DiscardSkip) m = new DiscardSkipMessage(b, this.GetPlayerIDs(), this.GameRules);
            else if (b[0] == (byte)pxMessages.DialogMessage) m = new DialogMessage(b);
            else if (b[0] == (byte)pxMessages.ErrorMessage) m = new ErrorMessage(b);
            else if (b[0] == (byte)pxMessages.GameOver) m = new GameOverMessage(b);
            else if (b[0] == (byte)pxMessages.GameRules) m = new GameRulesMessage(b);
            else if (b[0] == (byte)pxMessages.GameStarting) m = new GameStartingMessage(b);
            else if (b[0] == (byte)pxMessages.Goodbye) m = new GoodbyeMessage(b);
            else if (b[0] == (byte)pxMessages.GotCards) m = new GotCardsMessage(b, this.GameRules);
            else if (b[0] == (byte)pxMessages.GotDeckCard) m = new GotDeckCardMessage(b, this.GetPlayerIDs());
            else if (b[0] == (byte)pxMessages.GotDiscard) m = new GotDiscardMessage(b, this.GetPlayerIDs(), this.GameRules);
            else if (b[0] == (byte)pxMessages.Hand) m = new HandMessage(b, this.GameRules);
            else if (b[0] == (byte)pxMessages.Login) m = new LoginMessage(b);
            else if (b[0] == (byte)pxMessages.LoginAcknowledgment) m = new LoginAckMessage(b);
            else if (b[0] == (byte)pxMessages.LogOff) m = new LogOffMessage(b, this.GetPlayerIDs());
            else if (b[0] == (byte)pxMessages.NewHand) m = new NewHandMessage(b);
            else if (b[0] == (byte)pxMessages.PlayedCardOnTable) m = new PlayedCardOnTableMessage(b, this.GetPlayerIDs(), this.GameRules);
            else if (b[0] == (byte)pxMessages.Ready) m = new ReadyMessage(b, this.GetPlayerIDs());
            else if (b[0] == (byte)pxMessages.Scoreboard) m = new ScoreboardMessage(b, this.GetPlayerIDs(), this.GameRules);
            else if (b[0] == (byte)pxMessages.SkipNotification) m = new SkipNotificationMessage(b, this.GetPlayerIDs());
            else if (b[0] == (byte)pxMessages.StartGameTimer) m = new StartGameTimerMessage(b);
            else if (b[0] == (byte)pxMessages.Status) m = new StatusMessage(b, this.GetPlayerIDs(), this.GameRules);
            else if (b[0] == (byte)pxMessages.SystemMessage) m = new SystemMessage(b);
            else if (b[0] == (byte)pxMessages.Table) m = new TableMessage(b, this.GameRules);
            else if (b[0] == (byte)pxMessages.TurnEnd) m = new TurnEndMessage(b, this.GetPlayerIDs());
            else if (b[0] == (byte)pxMessages.TurnStart) m = new TurnStartMessage(b, this.GetPlayerIDs());
            else if (b[0] == (byte)pxMessages.UpdateDiscard) m = new UpdateDiscardMessage(b, this.GetPlayerIDs(), this.GameRules);
            else if (b[0] == (byte)pxMessages.WentOut) m = new WentOutMessage(b, this.GetPlayerIDs());
            else if (b[0] == (byte)pxMessages.Won) m = new WonMessage(b, this.GetPlayerIDs());
            else m = new UnknownMessage(b);          
            
            return m;
		}
		
		public void Login(int major, int minor, int build, string client_title)
		{
			if (_loggedIn) return;
			_loggedIn = false;
			Send(new LoginMessage(
				this.playerSettings._Name
				, 0
				, _readyToPlay
                , _computerPlayer
				, true
				, major
				, minor
				, build
				, client_title
				, System.Environment.Version.ToString()
				, System.Environment.OSVersion.ToString()
				, GameLibraryVersion.Major
				, GameLibraryVersion.Minor
				, GameLibraryVersion.Build));

            //start timeout	
			new System.Threading.Timer
			(
				new TimerCallback(this.LoginAckTimeout)
				, null
				, TimingDefinitions.LoginTimeout_Client
				, Timeout.Infinite
			);
			

			

			//go through all messages received and wait for a login acknowledgement
			Message m = null;
			bool login_acknowledged = false;
			while ((!login_acknowledged) && (_connected))
			{
                try
                {
                    m = this.getMessage();
                    if (m == null)
                    {
                        Thread.Sleep(TimingDefinitions.GetMessage_Client);
                        continue;
                    }
                    if (m.GetType().Equals(typeof(LoginAckMessage)))
                    {
                        LoginAckMessage lam = (LoginAckMessage)m;
                        lock (this)
                        {
                            _loggedIn = true;
                            this._playerID = lam.Id;
                        }
                        login_acknowledged = true;
                        
                    }
                }
                catch (BadMessageException bme)
                {
                    PhazeXLog.LogError(bme, "Client", 0);
                    login_acknowledged = false;
                }
			}
			
			if ((!_connected) || (!login_acknowledged))
			{
				throw new LoginException("Login not acknowledged within " + TimingDefinitions.LoginTimeout_Client + "ms. Server may have died?");
			}


			
			Player me = new Player(false);
            me.Name = this.playerSettings._Name;
			me.PlayerID = _playerID;
			me.Release = major;
			me.Revision = minor;
			me.Build = build;
			me.ClientTitle = client_title;
			me.LibRelease = GameLibraryVersion.Major;
			me.LibRevision = GameLibraryVersion.Minor;
			me.LibBuild = GameLibraryVersion.Build;
			me.FrameworkVersion = System.Environment.Version.ToString();
			me.OSVersion = System.Environment.OSVersion.ToString();
			AddPlayer(me);
			return;
		}
		public void LoginAckTimeout(object o)
		{
			//this function is called when the server doesn't respond in some number of seconds!
			lock(this)
			{
				if (!_loggedIn)
				{
                    PhazeXLog.LogError(
                        new Exception("Didn't get login acknowledgement in " + TimingDefinitions.LoginTimeout_Client + " ms")
                        , GameLibraryVersion.VersionString, 0);
					this.Disconnect();
				}
			}
		}
		#endregion
        #region basic network request functions
        protected void readyToPlay(bool b)
		{
			if (_gameStarted) return;
			if (_readyToPlay != b)
			{
				lock(this)
				{
					_readyToPlay = b;
				}
				Send(new ReadyMessage(_playerID, _readyToPlay));
			}
        }
        protected void getDiscard()
        {
            if (!_gameStarted) return;
            Send(new GetTopDiscardMessage());
        }
        protected void getDeckCard()
        {
            if (!_gameStarted) return;
            Send(new GetTopDeckCardMessage());
        }
        #endregion
        #region basic helper functions
        public PhazeRule GetCurrentPhazeRule()
        {
            return this.GameRules.PhazeRules.Phaze(_currentPhazeRuleNumber);
        }
        #endregion
        #region Players
        public List<Player> GetPlayers()
        {
            List<Player> retval = null;
            lock(_players)
            {
                retval = new List<Player>(_players);
            }
            return retval;
        }
        public List<Player> GetOtherPlayers()
        {
            List<Player> retval = null;
            lock (_players)
            {
                retval = new List<Player>(_players.Length - 1);
                foreach (Player p in _players)
                {
                    if (p.PlayerID != this._playerID)
                    {
                        retval.Add(p);
                    }
                }
            }
            return retval;
        }
		public void RemovePlayer(Player p)
		{
			if (_players == null) return;
			lock(this)
			{
				bool found = false;
				foreach (Player player in _players)
				{
					if (p.PlayerID == player.PlayerID)
					{
						found = true;
					}
				}
				if (found)
				{
					int np_ctr = 0;
					int p_ctr = 0;
					Player [] new_players = new Player[_players.Length - 1];
					foreach (Player player in _players)
					{
						if (p.PlayerID != player.PlayerID)
						{								
							new_players[np_ctr] = _players[p_ctr];
							np_ctr++;
						}
						p_ctr++;
					}
					_players = new_players;
				}
			}
			return;
		}
		public int GetPlayerCount()
		{
			if (_players == null) return 0;
			int retval = 0;
			lock(this)
			{
				retval = _players.Length;
			}
			return retval;
		}
		public int GetConnectedPlayerCount()
		{
			if (_players == null) return 0;
			int retval = 0;
			lock(this)
			{
				foreach (Player p in _players)
				{
					if (p.Connected) retval++;
				}
			}
			return retval;
		}
		public Player GetPlayerAtPos(int pos)
		{
			if (_players == null) return null;
			if (_players.Length <= pos) return null;
			return _players[pos];
		}
        public int GetPlayerPosWithID(int id)
        {
            if (_players == null) return -1;
            int retval = -1;
            lock (this)
            {
                for (int ctr = 0; ctr < _players.Length; ctr++)
                {
                    if (_players[ctr].PlayerID == id)
                    {
                        retval = ctr;
                        break;
                    }
                }
            }
            return retval;
        }
		public Player GetPlayerWithID(int id)
		{
			if (_players == null) return null;
			Player retval = null;
			lock(this)
			{
				foreach (Player p in _players)
				{
					if (p.PlayerID == id)
					{
						retval = p;
						break;
					}
				}
			}
			return retval;
		}
		public void AddPlayer(Player p)
		{
			lock(this)
			{
				if (_players == null)
				{
					_players = new Player[1];
					_players[0] = p;
				}
				else
				{
					//TODO
					//add the player in order!
					Player [] _players_new = new Player[_players.Length + 1];
					int ctr = 0;
					foreach (Player player in _players)
					{
						_players_new[ctr] = player;
						ctr++;
					}
					_players_new[ctr] = p;
					_players = _players_new;
				}
			}
		}
		public Player FindPlayer(int id, bool makenew)
		{
			if (_players == null) return null;
			Player retval = null;
			foreach (Player p in _players)
			{
				if (p.PlayerID == id)
				{
					retval = p;
					break;
				}
			}

			if ((retval == null) && (makenew))
			{
				Player p = new Player(false);
				p.Name = "[Player " + id + "]";
				p.PlayerID = id;
				AddPlayer(p);
				retval = p;
			}


			return retval;
		}
		public int [] GetPlayerIDs()
		{
			int [] ids;
			if (_players == null)
			{
				ids = new int[1];
				ids[0] = -1;
				return ids;
			}
			
			lock(this)
			{
				int ctr = 0;
				ids = new int[_players.Length];
				foreach (Player p in _players)
				{
					ids[ctr] = p.PlayerID;
					ctr++;
				}
			}
			return ids;
		}
        public void UpdateHand()
        {
            lock (this.socketSync)
            {
                Send(new RequestHandMessage());
            }
        }
        public void UpdateTable()
        {
            lock (this.socketSync)
            {
                Send(new RequestTableMessage());
            }
        }
        public void Meld(List<Group> melds, int currentPhazeNumber)
        {
            lock (this.socketSync)
            {
                Send(new MeldMessage(melds, currentPhazeNumber));
            }
        }
        
		#endregion
		#region Process Messages
		public void ProcessMessages()
		{
            Message m = null;
			while (_connected)
			{
                try
                {
                    m = getMessage();
                }
                catch (BadMessageException bme)
                {
                    _mp.ProcessBadMessage(bme);
                }
				if (m == null)
				{
                    Thread.Sleep(TimingDefinitions.ListenSocket_Client);
				}
				else
				{
					processMessage(m);
				}
			}

			return;
		}
		protected virtual void processMessage(Message m)
		{
            if (m == null) return;
            else if (m.GetType().Equals(typeof(HeartBeatMessage))) _mp.ProcessHeartbeatMessage((HeartBeatMessage)m);
            else if (m.GetType().Equals(typeof(ChangeNameMessage))) _mp.ProcessChangeNameMessage((ChangeNameMessage)m);
            else if (m.GetType().Equals(typeof(ChangeNameRejectMessage))) _mp.ProcessChangeNameRejectMessage((ChangeNameRejectMessage)m);
            else if (m.GetType().Equals(typeof(ChatMessage)))_mp.ProcessChatMessage((ChatMessage)m);
            else if (m.GetType().Equals(typeof(CompletedPhazeMessage))) _mp.ProcessCompletedPhazeMessage((CompletedPhazeMessage)m);
            else if (m.GetType().Equals(typeof(CurrentPhazeMessage))) _mp.ProcessCurrentPhazeMessage((CurrentPhazeMessage)m);
            else if (m.GetType().Equals(typeof(DiscardSkipMessage))) _mp.ProcessDiscardSkipMessage((DiscardSkipMessage)m);
            else if (m.GetType().Equals(typeof(DialogMessage))) _mp.ProcessDialogMessage((DialogMessage)m);
            else if (m.GetType().Equals(typeof(ErrorMessage))) _mp.ProcessErrorMessage((ErrorMessage)m);
            else if (m.GetType().Equals(typeof(GameOverMessage))) _mp.ProcessGameOverMessage((GameOverMessage)m);
            else if (m.GetType().Equals(typeof(GameRulesMessage))) _mp.ProcessGameRulesMessage((GameRulesMessage)m);
            else if (m.GetType().Equals(typeof(GameStartingMessage))) _mp.ProcessGameStartingMessage((GameStartingMessage)m);
            else if (m.GetType().Equals(typeof(GoodbyeMessage))) _mp.ProcessGoodbyeMessage((GoodbyeMessage)m);
            else if (m.GetType().Equals(typeof(GotCardsMessage))) _mp.ProcessGotCardsMessage((GotCardsMessage)m);
            else if (m.GetType().Equals(typeof(GotDeckCardMessage))) _mp.ProcessGotDeckCardMessage((GotDeckCardMessage)m);
            else if (m.GetType().Equals(typeof(GotDiscardMessage))) _mp.ProcessGotDiscardMessage((GotDiscardMessage)m);
            else if (m.GetType().Equals(typeof(HandMessage))) _mp.ProcessHandMessage((HandMessage)m);
            else if (m.GetType().Equals(typeof(LoginMessage))) _mp.ProcessLoginMessage((LoginMessage)m);
            else if (m.GetType().Equals(typeof(LogOffMessage))) _mp.ProcessLogOffMessage((LogOffMessage)m);
            else if (m.GetType().Equals(typeof(NewHandMessage))) _mp.ProcessNewHandMessage((NewHandMessage)m);
            else if (m.GetType().Equals(typeof(PlayedCardOnTableMessage))) _mp.ProcessPlayedCardOnTableMessage((PlayedCardOnTableMessage)m);
            else if (m.GetType().Equals(typeof(ReadyMessage))) _mp.ProcessReadyMessage((ReadyMessage)m);
            else if (m.GetType().Equals(typeof(ScoreboardMessage))) _mp.ProcessScoreboardMessage((ScoreboardMessage)m);
            else if (m.GetType().Equals(typeof(SkipNotificationMessage))) _mp.ProcessSkipNotificationMessage((SkipNotificationMessage)m);
            else if (m.GetType().Equals(typeof(StartGameTimerMessage))) _mp.ProcessStartGameTimerMessage((StartGameTimerMessage)m);
            else if (m.GetType().Equals(typeof(StatusMessage))) _mp.ProcessStatusMessage((StatusMessage)m);
            else if (m.GetType().Equals(typeof(SystemMessage))) _mp.ProcessSystemMessage((SystemMessage)m);
            else if (m.GetType().Equals(typeof(TableMessage))) _mp.ProcessTableMessage((TableMessage)m);
            else if (m.GetType().Equals(typeof(TurnEndMessage))) _mp.ProcessTurnEndMessage((TurnEndMessage)m);
            else if (m.GetType().Equals(typeof(TurnStartMessage))) _mp.ProcessTurnStartMessage((TurnStartMessage)m);
            else if (m.GetType().Equals(typeof(UpdateDiscardMessage))) _mp.ProcessUpdateDiscardMessage((UpdateDiscardMessage)m);
            else if (m.GetType().Equals(typeof(WentOutMessage))) _mp.ProcessWentOutMessage((WentOutMessage)m);
            else if (m.GetType().Equals(typeof(WonMessage))) _mp.ProcessWonMessage((WonMessage)m);
            else _mp.ProcessUnknownMessage((UnknownMessage)m);
			return;
		}
		#endregion
	}
}
