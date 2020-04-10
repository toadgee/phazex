using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using PhazeX.Helpers;
using PhazeX.Network.Messages;
using PhazeX.Options;


namespace PhazeX.Network.Server
{
    

	public class ServerGame
	{
        public const int InitialStartGameTimerSeconds = 10;
        private const string pid = "PhazeX.Network.Server.ServerGame";
        private List<PlayerMessage> _messagesToProcess;
		private RingList _players;
		private RulesServer _rulesServer;
		private ConnServer _connServer;
		private bool _process;
		private bool _startGameTimerStarted;
        protected bool _useStartGameTimer = true;

        private int _unusedPlayerID;
		private int _handNumber;
        public int HandNumber { get { return _handNumber; } }
		private int _turn;
        private int _totalTurns;
        public int TotalTurns { get { return _totalTurns; } }
		protected int _startGameTimerSeconds;

        private bool _shutdown;
	    private bool _initialized;
		private bool _gameStarted;
        public bool GameStarted { get { return _gameStarted; } }
		private bool _gameOver;
		private bool _firstDiscard;

		private Deck _deck;
		private Table _table;
		private Discard _discard;

        private GameRules rules;

        private object _bytesLock = new object();
        private ulong _bytesSent = 0;
        private ulong _bytesReceived = 0;
        public ulong BytesSent { get { return _bytesSent; } }
        public ulong BytesReceived { get { return _bytesReceived; } }

        public bool HasShutdown { get { return _shutdown; } }

		public ServerGame(GameRules rules, NetworkSettings settings)
		{
			lock(this)
			{
				_process = true;
				_initialized = false;
				_startGameTimerStarted = false;
			}
            this.rules = rules;

            _messagesToProcess = new List<PlayerMessage>(1024);
			_players = new RingList();
			_unusedPlayerID = 0;
			_handNumber = 0;
			_turn = 0;
            _totalTurns = 1;
			_startGameTimerSeconds = InitialStartGameTimerSeconds;
            _shutdown = false;

			_deck = null;
			_table = null;
			_discard = null;

			_gameStarted = false;
			_gameOver = false;

			Thread t;

			//start rules Server!
			_rulesServer = new RulesServer();
			_connServer = new ConnServer(this, this.rules, settings);
			
			if ((!_rulesServer.IsBound()) || (!_connServer.IsBound()))
			{
				_rulesServer.Unbind();
				_connServer.Stop();
				return;
			}
			else
			{
				lock(this)
				{
					_initialized = true;
				}
			}
					

			t = new Thread(new ThreadStart(_rulesServer.Start));
			t.Name = "PXServer Rules Server";
            //t.IsBackground = true;
			t.Start();

			t = new Thread(new ThreadStart(_connServer.Start));
			t.Name = "PXServer Connection Server";
            //t.IsBackground = true;
			t.Start();

			t = new Thread(new ThreadStart(this.process));
			t.Name = "PXServer Message Processing thread";
            //t.IsBackground = true;
			t.Start();

			t = new Thread(new ThreadStart(this.doHeartBeat));
			t.Name = "Heartbeat thread";
            //t.IsBackground = true;
			t.Start();

		}
        public void SentBytes(ulong bytes)
        {
            lock (_bytesLock)
            {
                _bytesSent += bytes;
            }
        }
        public void ReceivedBytes(ulong bytes)
        {
            lock (_bytesLock)
            {
                _bytesReceived += bytes;
            }
        }
        public void AddMessageToProcess(ServerPlayer sp, byte [] b)
        {
            if (b.Length < 1)
            {
                PhazeXLog.LogError
                (
                     new Exception("Attempted to process 0 byte message!")
                    , GameLibraryVersion.ProgramIdentifier
                    , 0
                );
            }
            byte start = b[0];

            try
            {
                Message msg = null;
                switch (start)
                {
                    case (byte)pxMessages.Heartbeat:
                        msg = new HeartBeatMessage(b);
                        break;
                    case (byte)pxMessages.SystemMessage:
                        msg = new SystemMessage(b);
                        break;
                    case (byte)pxMessages.Chat:
                        msg = new ChatMessage(b, this.getIDs());
                        break;
                    case (byte)pxMessages.Ready:
                        msg = new ReadyMessage(b, this.getIDs());
                        break;
                    case (byte)pxMessages.ChangeName:
                        msg = new ChangeNameMessage(b, this.getIDs());
                        break;
                    case (byte)pxMessages.PlayOnGroup:
                        msg = new PlayOnGroupMessage(b, this.rules);
                        break;
                    case (byte)pxMessages.Meld:
                        msg = new MeldMessage(b, this.rules);
                        break;
                    case (byte)pxMessages.GetTopDiscard:
                        msg = new GetTopDiscardMessage(b);
                        break;
                    case (byte)pxMessages.GetTopDeckCard:
                        msg = new GetTopDeckCardMessage(b);
                        break;
                    case (byte)pxMessages.DiscardSkip:
                        msg = new DiscardSkipMessage(b, this.getIDs(), this.rules);
                        break;
                    case (byte)pxMessages.Discard:
                        msg = new DiscardMessage(b, this.rules);
                        break;
                    case (byte)pxMessages.RequestHand:
                        msg = new RequestHandMessage(b);
                        break;
                    case (byte)pxMessages.RequestTable:
                        msg = new RequestTableMessage(b);
                        break;
                    default:
                        msg = new UnknownMessage(b);
                        break;
                }
                lock (this)
                {
                    _messagesToProcess.Add(new PlayerMessage(sp, msg));
                }
            }
            catch (BadMessageException bme)
            {
                PhazeXLog.LogError
                (
                    bme
                    , GameLibraryVersion.ProgramIdentifier
                    , 109
                );

                string info = "";
                foreach (byte tmp in b)
                {
                    info += ((int)tmp).ToString() + " ";
                }
                PhazeXLog.LogInformation(info, pid);
            }
        }
        private PlayerMessage getMessageToProcess()
        {
           PlayerMessage retval = null;
           lock (this)
           {
               if (_messagesToProcess.Count > 0)
               {
                   retval = _messagesToProcess[0];
                   _messagesToProcess.RemoveAt(0);
               }
           }
           return retval;
        }
		public int PlayerCount()
		{
			int i = 0;
			lock(this)
			{
				i = _players.Count;
			}
			return i;
		}
		public bool IsProcessing()
		{
			return _process;
		}
		public bool Started()
		{
			return _initialized;
		}
		private void doHeartBeat()
		{
			ServerPlayer p;
			while (_process)
			{
				lock(this)
				{
					foreach (object o in _players)
					{
						p = (ServerPlayer)o;
						if (!p.Connected) continue;
						p.CheckConnectivity();
					}
				}
				Thread.Sleep(TimingDefinitions.Heartbeat_Server);
			}
		}
		private void process()
		{
			int ctr;
			ServerPlayer p;
            PlayerMessage pm;
			while (_process)
			{
                pm = getMessageToProcess();
                if (pm == null)
                {
                    Thread.Sleep(TimingDefinitions.GetMessage_Server);
                    continue;
                }
                while (pm != null)
                {
                    this.handleMessage(pm.MessageObj, pm.Player);
                    pm = getMessageToProcess();
                }

				lock(this)
				{
					if (_gameStarted)
					{
						for (ctr = 0; ctr < _players.Count; ctr++)
						{
							p = (ServerPlayer)_players[ctr];
							if (p.ShouldRemoveMe())
							{
								this.PlayerDisconnected(p);
							}
						}
					}
					else
					{
						for (ctr = 0; ctr < _players.Count; ctr++)
						{
							p = (ServerPlayer)_players[ctr];
							if (p.ShouldRemoveMe())
							{
								this.PlayerDisconnected(p);
								ctr--;
							}
						}
					}
				}
			}
		}
		#region Player functions
		public void AddPlayer(ServerPlayer p)
		{
			_players.Add(p);
		}
		public ServerPlayer NextPlayer(ServerPlayer p)
		{
			return (ServerPlayer)_players.NextObject(p);
		}
		public void LoginPlayer(ServerPlayer p, LoginMessage lm)
		{
			lock(this)
			{
                p.Name = lm.Username;
                p.ComputerPlayer = lm.ComputerPlayer;
                p.Release = lm.Major;
                p.Revision = lm.Minor;
                p.Build = lm.Build;
                p.LibRelease = lm.Lib_Major;
                p.LibRevision = lm.Lib_Minor;
                p.LibBuild = lm.Lib_Build;
                p.FrameworkVersion = lm.Framework;
                p.OSVersion = lm.Os;
                p.ClientTitle = lm.Title;
				
				p.PlayerID = _unusedPlayerID;
				_unusedPlayerID++;
                _players.Add(p);

				p.Send(new LoginAckMessage(p.PlayerID));

				p.Send(new GameRulesMessage(this.rules));

				//let the player know who all is here already
				foreach (object o in _players)
				{
					if (p != (ServerPlayer)o)
					{
                        ServerPlayer sp = (ServerPlayer)o;
						p.Send(new LoginMessage(
							  sp.Name
							, sp.PlayerID
							, sp.ReadyToPlay
                            , sp.ComputerPlayer
							, false
							, sp.Release
							, sp.Revision
							, sp.Build
							, sp.ClientTitle
							, sp.FrameworkVersion
							, sp.OSVersion
							, sp.LibRelease
							, sp.LibRevision
							, sp.LibBuild));
					}
				}
                this.SendAll(new LoginMessage(p.Name, p.PlayerID, p.ReadyToPlay, p.ComputerPlayer
					, true, p.Release, p.Revision, p.Build, p.ClientTitle, p.FrameworkVersion
					, p.OSVersion, p.LibRelease, p.LibRevision, p.LibBuild), p);
			}
		}
		private ServerPlayer findPlayer(int id)
		{
			ServerPlayer p = null;
			lock(this)
			{
				foreach (object o in _players)
				{
					if (((ServerPlayer)o).PlayerID == id)
					{
						p = (ServerPlayer)o;
					}
				}
			}
			return p;
		}
		#endregion
		#region Network functions
		public void SendAll(Message msg, ServerPlayer p)
		{
            lock (this)
            {
                foreach (object o in _players)
                {
                    if (o.Equals(p)) continue; //don't send to the player specified
                    ServerPlayer sp = (ServerPlayer)o;
                    sp.Send(msg);
                }
            }
		}
		public void SendAll(Message msg)
		{
			SendAll(msg, null);
		}
		#endregion
		#region disconnect function
		public void PlayerDisconnected(ServerPlayer p)
		{
			if (!_gameStarted)
			{
				SendAll(new LogOffMessage(p.PlayerID, true));
				lock(this)
				{
					_players.Remove(p);
				}
				return;
			}

			if (!_gameOver)
			{
                SendAll(new LogOffMessage(p.PlayerID, false));

				lock(this)
				{
					//put players cards back in deck
					Hand h = p.GetHand();
					if (h != null)
					{
						if (h.Count > 0)
						{
                            List<Card> addBack = new List<Card>();
							foreach (Card c in h)
							{
								h.Remove(c);
                                addBack.Add(c);
							}
							_deck.ShuffleAndAddUnused(addBack);
						}
					}


					
					
					
					//check to see how many players are left
					int ctr = 0;
					foreach (object o in _players)
					{
						if (((ServerPlayer)o).Connected)
						{
							ctr++;
						}
					}
					//if only 1 player is left (and the game is started) end the game
					if ((ctr == 1) && (_gameStarted))
					{
						EndGame();
					}
					p.RemovedFromGame();
				}
				if (p.MyTurn)
				{
					//TODO: add better logic!
					this.endTurn(p, true);
				}
			}
		}
		#endregion
		#region Start / End functions
		
		public virtual void EndGame()
		{
			//determine who won
			ArrayList topPhazeList = new ArrayList();
			int topPhazeNum = 0;
			ServerPlayer p;
			
			//stop processing!
			lock(this)
			{
				_process = false;
			}

			if (_gameStarted)
			{
				lock(this)
				{
					_gameOver = true;
				}

				foreach (object o in _players)
				{
					p = (ServerPlayer)o;
					Scoreboard s = p.Scoreboard;
					if (s.CompletedPhazeNumber > topPhazeNum)
					{
						topPhazeList = new ArrayList();
						topPhazeList.Add(p);
						topPhazeNum = s.CompletedPhazeNumber;
					}
                    else if (s.CompletedPhazeNumber == topPhazeNum)
					{
						topPhazeList.Add(p);
					}
				}
			
				ArrayList winningPlayers = new ArrayList();
				int leastPoints = 0;
				if (topPhazeList.Count > 1)
				{
					p = (ServerPlayer)topPhazeList[0];
					//determine who has the least points!
					leastPoints = p.Scoreboard.Points;
					for (int ctr = 0; ctr < topPhazeList.Count; ctr++)
					{
						p = (ServerPlayer)topPhazeList[ctr];
						if (p.Scoreboard.Points < leastPoints)
						{
							winningPlayers = new ArrayList();
							winningPlayers.Add(p);
							leastPoints = p.Scoreboard.Points;
						}
						else if (p.Scoreboard.Points == leastPoints)
						{
							winningPlayers.Add(p);
						}
					}
				}
				else if (topPhazeList.Count == 1)
				{
					p = (ServerPlayer)topPhazeList[0];
					winningPlayers.Add(p);
					leastPoints = p.Scoreboard.Points;
				}

				int [] winners = new int[winningPlayers.Count];
				foreach (object o in winningPlayers)
				{
					winners[winningPlayers.IndexOf(o)] = ((ServerPlayer)o).PlayerID;
				}
                
                SendAll(new WonMessage(winners, leastPoints));
			
			
				updateClientsScoreboards();
				SendAll(new GameOverMessage());
			}
			Cancel();
		}
        public virtual void StartHand()
		{
			_deck = new Deck(this.rules);
			_table = new Table();
			_discard = new Discard();
			_firstDiscard = true;

			ServerPlayer p;
			PhazeRule pr;
			int prnum;
			bool dealt;

			//this does [X] things:
			//tell the player what phaze they're on
			//create a new hand for them (and notify them)
			//create a scoreboard for them
			foreach (object o in _players)
			{
				p = (ServerPlayer)o;
				dealt = false;
				if (p == (ServerPlayer)_players[_handNumber])
				{
					dealt = true;
				}
				prnum = p.CurrentPhaze;
				pr = this.rules.PhazeRules.Phaze(prnum);
				//notify the client what phaze they're on
				
                p.Send(new CurrentPhazeMessage(prnum));

				p.NewHand(pr);
                p.Send(new NewHandMessage());
				p.Scoreboard.AddSession(new Session(dealt, prnum));
			}
			deal();

			//reset turn
			_turn = 0;

			//update all statuses!
			updateClientsStatuses();
			updateClientsScoreboards();

			//tell the user whose turn it is!
			p = (ServerPlayer)_players[_handNumber];
			p.MyTurn = true;
			p.PickedUpCard = false; //double check!
            startTurn(p);
		}
		public virtual void EndHand(Player currentPlayer, Card finalCard)
		{
            SendAll(new UpdateDiscardMessage(finalCard, currentPlayer, true));
            SendAll(new TableMessage(new Table()));

			//determine score
			ServerPlayer p;
			Session s;
			bool endOfGame = false;
			foreach (object o in _players)
			{
				p = (ServerPlayer)o;
				s = p.Scoreboard[_handNumber];
				s.MadePhaze = p.CompletedPhaze;
				//increment phaze number, if necessary
				if (p.CompletedPhaze)
				{
					p.CurrentPhaze++;
				}
                if (p.CurrentPhaze == this.rules.PhazeRules.Count())
				{
					endOfGame = true;
				}
				
				//card points!
				foreach (Card c in p.GetHand())
				{
                    s.Points += this.rules.GetCardPointValue(c.Number);
				}
								
				//reset player attributes
				p.CompletedPhaze = false;
				while (p.IsSkipped)
				{
					p.Skipped();
				}
				p.MyTurn = false;
				p.PickedUpCard = false;
			}
			updateClientsScoreboards();
			
			
			//check for end of game requirements
			if (endOfGame)
			{
				EndGame();
			}
			else
			{
				_handNumber++;
				_turn = 0;
				StartHand();
			}
		}
        protected virtual void startTurn(ServerPlayer p)
        {
            SendAll(new TurnStartMessage(p.PlayerID));
        }
		private void endTurn(ServerPlayer p, bool nextPlayer)
		{
			p.MyTurn = false;
			p.PickedUpCard = false;
            SendAll(new TurnEndMessage(p.PlayerID));
			updateClientsStatuses();

            if (!nextPlayer) return;

			//increment turn!
			_turn++;
            _totalTurns++;

			//find new player turn!
			bool found = false;
			ServerPlayer firstPlayer = null;
			while (!found)
			{
				p = (ServerPlayer)_players[_handNumber + _turn];
				if (firstPlayer == null)
				{
					firstPlayer = p;
				}
				else if (firstPlayer == p)
				{
					return;
				}

				found = true;
				if (p.IsSkipped)
				{
					found = false;
					p.Skipped();
                    SendAll(new SkipNotificationMessage(p.PlayerID));
					_turn++;
                    _totalTurns++;
				}
				if (!p.Connected)
				{
					found = false;
					_turn++;
				}
			}
			p.MyTurn = true;
			p.PickedUpCard = false; //just for safety check
            startTurn(p);
		}
		public virtual void Cancel()
		{
            lock (this)
            {
                _process = false;
                _gameStarted = false;

                foreach (object o in _players)
                {
                    ServerPlayer p = (ServerPlayer)o;
                    if (p.Connected)
                    {
                        p.Send(new GoodbyeMessage());
                        p.Disconnect();
                    }
                }
                _rulesServer.Stop();

                _connServer.Stop();

                _shutdown = true;
            }
            
		}
		#endregion
		#region helpers functions
		private int [] getIDs()
		{
			int [] ids;
			lock(this)
			{
				int ctr = 0;
				ids = new int[_players.Count];
				foreach (object o in _players)
				{
					ServerPlayer p = (ServerPlayer)o;
					ids[ctr] = p.PlayerID;
					ctr++;
				}
			}
			return ids;
		}
		private void updateClientsStatuses()
		{
			ServerPlayer p;
			foreach (object o in _players)
			{
                if (o == null) continue;
				p = (ServerPlayer)o;

                Message sm = new StatusMessage
                (
                      p.PlayerID
				    , p.CompletedPhaze
                    , p.CurrentPhaze
				    , p.GetHand().Count
                    , p.SkipsLeft
                );
                SendAll(sm);
			}
		}
		private void updateClientsDiscard(Card c)
		{
			_discard.AddTopCard(c);
            SendAll(new UpdateDiscardMessage(c, null, false));
		}
		private void updateClientsHands()
		{
			ServerPlayer p;
			foreach (object o in _players)
			{
				p = (ServerPlayer)o;
                p.Send(new HandMessage(p.GetHand(), false));
			}
		}
		private void updateClientsScoreboards()
		{
			ServerPlayer p;
            lock (this)
            {
                foreach (object o in _players)
                {
                    p = (ServerPlayer)o;
                    SendAll(new ScoreboardMessage(p.Scoreboard, p.PlayerID));
                }
            }
		}
		private void deal()
		{
			ServerPlayer p;
			Card c;

			//virtually deal
            for (int ctr = 0; ctr < this.rules.HandCards; ctr++)
			{
				foreach (object o in _players)
				{
					p = (ServerPlayer)o;
					c = _deck.RemoveCard();
					p.AddHandCard(c);
				}
			}


			//tell all clients what they have!
			updateClientsHands();
			updateClientsDiscard(_deck.RemoveCard());
		}
		#endregion
		#region Handle Message Functions
		private void handleMessage(Message m, ServerPlayer p)
		{
			lock(this)
			{
                try
                {
                    if (m == null) return;
                    if (m.GetType().Equals(typeof(HeartBeatMessage))) handleHeartBeatMessage((HeartBeatMessage)m, p);
                    else if (m.GetType().Equals(typeof(SystemMessage))) handleSystemMessage((SystemMessage)m, p);
                    else if (m.GetType().Equals(typeof(ChatMessage))) handleChatMessage((ChatMessage)m, p);
                    else if (m.GetType().Equals(typeof(ReadyMessage))) handleReadyMessage((ReadyMessage)m, p);
                    else if (m.GetType().Equals(typeof(ChangeNameMessage))) handleChangeNameMessage((ChangeNameMessage)m, p);
                    else if (m.GetType().Equals(typeof(PlayOnGroupMessage))) handlePlayOnGroupMessage((PlayOnGroupMessage)m, p);
                    else if (m.GetType().Equals(typeof(MeldMessage))) handleMeldMessage((MeldMessage)m, p);
                    else if (m.GetType().Equals(typeof(GetTopDiscardMessage))) handleGetTopDiscardMessage((GetTopDiscardMessage)m, p);
                    else if (m.GetType().Equals(typeof(GetTopDeckCardMessage))) handleGetTopDeckCardMessage((GetTopDeckCardMessage)m, p);
                    else if (m.GetType().Equals(typeof(DiscardSkipMessage))) handleDiscardSkipMessage((DiscardSkipMessage)m, p);
                    else if (m.GetType().Equals(typeof(DiscardMessage))) handleDiscardMessage((DiscardMessage)m, p);
                    else if (m.GetType().Equals(typeof(RequestHandMessage))) handleRequestHandMessage((RequestHandMessage)m, p);
                    else if (m.GetType().Equals(typeof(RequestTableMessage))) handleRequestTableMessage((RequestTableMessage)m, p);
                    else
                    {
                        UnknownMessage um = (UnknownMessage)m;
                        PhazeXLog.LogError
                        (
                            new Exception("Unknown message from client received! [" + um.Identifier + "]")
                             , GameLibraryVersion.ProgramIdentifier
                             , 108
                        );
                    }
                }
                catch (BadMessageException bme)
                {
                    PhazeXLog.LogError
                        (
                             bme
                             , GameLibraryVersion.ProgramIdentifier
                             , 0
                        );
                }
			
			}
		}
		private void handleSystemMessage(SystemMessage m, ServerPlayer p)
		{
			//client tried to send system message
			//message should be logged somewhere!
			//but we will NOT pass it on to the clients
            //should we write the system message? sure, it's just a text file
            PhazeXLog.LogWarning("Client (" + p.Name + ") told the server a system message." + m.ToString(), GameLibraryVersion.ProgramIdentifier, 0);
			return;
		}
		private void handleHeartBeatMessage(HeartBeatMessage m, ServerPlayer p)
		{
		}
		private void handlePlayOnGroupMessage(PlayOnGroupMessage m, ServerPlayer p)
		{
			if (!p.MyTurn)
			{
                p.Send(new ErrorMessage("It is not your turn to play on the table!"));
                p.Send(new HandMessage(p.GetHand(), false));
				return; //client sent invalid request!
			}

            if (p.GetHand().RemoveCardWithId(m.CardObj.Id) == null)
			{
				//invalid request
				//send cards back, biotches!
				//todo:: error message
                p.Send(new ErrorMessage("You do not have that card in your hand to play on the table. Returning card. [" + m.CardObj.Id + "]"));
                p.Send(new HandMessage(p.GetHand(), true));
				return;
			}

			if (p.GetHand().Count == 0)
			{
				//invalid request!
                p.Send(new ErrorMessage("You must discard your last card! Returning card."));
				p.GetHand().Add(m.CardObj);
                p.Send(new HandMessage(p.GetHand(), true));
				return;
			}
								
			//find group on table with correct ID
			Group g = _table.FindGroup(m.GroupId);
			if (g == null) //invalid request
			{
				//send cards back, biotches!
				p.GetHand().Add(m.CardObj);
                p.Send(new ErrorMessage("Group ID does not exist. Returning card."));
                p.Send(new HandMessage(p.GetHand(), true));
                p.Send(new TableMessage(_table));
			}
			else if (!g.CheckWith(m.CardObj))
			{
				//send cards back, biotches!
				p.GetHand().Add(m.CardObj);
                p.Send(new ErrorMessage("Card (" + m.CardObj.ToString() + ") does not fit into group (" + g.Id + "). Returning card."));
                p.Send(new HandMessage(p.GetHand(), false));
			}
			else
			{
				_table.PlayCard(m.CardObj, m.GroupId);
                SendAll(new PlayedCardOnTableMessage(p.PlayerID, _table.ReadGroup(m.GroupId), m.CardObj));
                SendAll(new TableMessage(_table));
				updateClientsStatuses();
			}
		}
		private void handleReadyMessage(ReadyMessage m, ServerPlayer p)
		{
			if (_gameStarted) return; //client sent invalid request!
			lock(this)
			{
				p.ReadyToPlay = m.Ready;
			}

			//notify everybody
            SendAll(new ReadyMessage(p.PlayerID, p.ReadyToPlay));

			int ready_players = 0;
			int total_players = 0;
			ServerPlayer p2;
			lock(this)
			{
				foreach (object o in _players)
				{
					p2 = (ServerPlayer)o;
					if (p2.Connected)
					{
						total_players++;
						if (p2.ReadyToPlay)
						{
							ready_players++;
						}
					}
				}
			}


			if ((ready_players == total_players) 
				&& (total_players > 2)
				&& (!_gameStarted) 
				&& (!_startGameTimerStarted))
			{
				lock(this)
				{
					_startGameTimerSeconds = 0;
					_startGameTimerStarted = true;
				}
                // StartGame();
			}
			if ((ready_players > 1)
				&& (total_players > 1)
				&& (!_gameStarted)
				&& (!_startGameTimerStarted))
			{
				//Thread t = new Thread(new ThreadStart(this.WaitStartGame));
                //t.IsBackground = true;
                //t.Name = "Waiting to start the game thread";
                //lock(this)
                //{
                //    _startGameTimerStarted = true;
                //}
                //t.Start();
			}
		}
		private void handleRequestHandMessage(RequestHandMessage m, ServerPlayer p)
		{
            p.Send(new HandMessage(p.GetHand(), true));
		}
		private void handleRequestTableMessage(RequestTableMessage m, ServerPlayer p)
		{
            p.Send(new TableMessage(_table));
		}
		private void handleChangeNameMessage(ChangeNameMessage m, ServerPlayer p)
		{			
			//handle what happens if player
			//has already changed their name
			if (p.ChangedName)
			{
                p.Send(new ChangeNameRejectMessage());
			}
			else
			{
                SendAll(new ChangeNameMessage(
					p.PlayerID
					, m.NewName
					, p.Name));
				lock(this)
				{
					p.Name = m.NewName;
					p.ChangedName = true;
				}
			}
		}
		private void handleGetTopDiscardMessage(GetTopDiscardMessage m, ServerPlayer p)
		{
            if (!p.MyTurn)
            {
                p.Send(new ErrorMessage("ERROR: It is not your turn to get the top discard!"));
                return; //client sent invalid request!
            }
            if (_discard.ReadTopCard() == null)
            {
                SendAll(new ErrorMessage("Internal server error"));
                return;
            }
            if ((_discard.ReadTopCard().Number == CardNumber.Skip) && (!_firstDiscard))//skip card
			{
                p.Send(new ErrorMessage("ERROR: You asked for a skip card. This is an invalid move, and should have been caught by your client. Upgrade!"));
				return; //client sent invalid request
			}

			//remove card from discard
			Card c = _discard.RemoveTopCard();
				
			p.PickedUpCard = true;
			p.GetHand().Add(c);

            CardCollection cc = new CardCollection(1);
            cc.Add(c);
            p.Send(new GotCardsMessage(cc));
            SendAll(new UpdateDiscardMessage(_discard.ReadTopCard(), p, false));
            SendAll(new GotDiscardMessage(p.PlayerID, c));
            updateClientsStatuses();
		}
		private void handleGetTopDeckCardMessage(GetTopDeckCardMessage m, ServerPlayer p)
		{
			if (!p.MyTurn) return; //client sent invalid request!

			//remove card from deck
			Card c = _deck.RemoveCard();

			//check to see if top deck card is null!
			if (_deck.IsTopCardNull())
			{
				_deck.ShuffleAndAddUnused(_discard.RemoveAllButTopCard());
			}
				
			//add to hand of client
			p.PickedUpCard = true;
			p.GetHand().Add(c);
            CardCollection cc = new CardCollection(1);
            cc.Add(c);
            p.Send(new GotCardsMessage(cc));

			//notify the users
            SendAll(new GotDeckCardMessage(p.PlayerID));
            updateClientsStatuses();
		}
		private void handleDiscardSkipMessage(DiscardSkipMessage m, ServerPlayer p)
		{
			if (!p.MyTurn)
			{
				//invalid request
                p.Send(new ErrorMessage("It is not your turn to discard a skip card!"));
				return;
			}
			
			if (!p.PickedUpCard)
			{
				//invalid request
                p.Send(new ErrorMessage("You did not yet pick up a card from the discard pile or deck!"));
				return;
			}

            if (p.GetHand().RemoveCardWithId(m.CardObj.Id) == null)
			{
				//invalid request
				//send cards back, biotches!
                p.Send(new ErrorMessage("You do not have the skip card in your hand. Returning card. [" + m.CardObj.Id + "]"));
                p.Send(new HandMessage(p.GetHand(), true));
				return;
			}
			
			if (p.GetHand().Count == 0)
			{
				endTurn(p, false);
                SendAll(new WentOutMessage(p.PlayerID));
				EndHand(p, m.CardObj);
				return;
			}
				
			ServerPlayer playerToSkip = findPlayer(m.PlayerId);
			if (playerToSkip == null)
			{
				//logError("No player with specified ID!");
                p.Send(new ErrorMessage("No player with specified ID! (" + m.PlayerId + ")"));
				p.AddHandCard(m.CardObj);
                p.Send(new HandMessage(p.GetHand(), false));
				return; //invalid request!
			}
			if (!playerToSkip.Connected)
			{
				p.AddHandCard(m.CardObj);
                p.Send(new ErrorMessage("Unable to skip player; player is no longer playing."));
                p.Send(new HandMessage(p.GetHand(), false));
				return; //invalid request
			}
			
			//make sure we can't pick any skip cards up
			_firstDiscard = false;
			
			_discard.AddTopCard(m.CardObj);
            SendAll(new UpdateDiscardMessage(m.CardObj, p, true));

			
			playerToSkip.SkippedBy(p);
			p.Skip(playerToSkip);
            SendAll(new DiscardSkipMessage(m.CardObj, playerToSkip.PlayerID));
            updateClientsStatuses();
			//update scoreboards!
			updateClientsScoreboards();

            Thread.Sleep(TimingDefinitions.pxcpArtificalWait);
            endTurn(p, true);
		}
		private void handleDiscardMessage(DiscardMessage m, ServerPlayer p)
		{
			if (!p.MyTurn)
			{
				//invalid request!
                p.Send(new ErrorMessage("It is not your turn to discard!"));
				return; //client sent invalid request!
			}
			
			if (!p.PickedUpCard)
			{
				//invalid request
                p.Send(new ErrorMessage("You did not yet pick up a card from the discard pile or deck!"));
				return;
			}

            if (p.GetHand().RemoveCardWithId(m.CardObj.Id) == null)
			{
				//invalid request
				//send cards back, biotches!
				p.AddHandCard(m.CardObj);
                p.Send(new ErrorMessage("You do not have the discard in your hand. Returning card. [" + m.CardObj.Id + "]"));
                p.Send(new HandMessage(p.GetHand(), true));
				return;
			}
			
			//make sure we can't pick any skip cards up
			_firstDiscard = false;
			
			if (p.GetHand().Count == 0)
			{
				endTurn(p, false);
                SendAll(new WentOutMessage(p.PlayerID));
				EndHand(p, m.CardObj);
				return;
			}
				
			_discard.AddTopCard(m.CardObj);
            SendAll(new UpdateDiscardMessage(m.CardObj, p, true));
            updateClientsStatuses();
            Thread.Sleep(TimingDefinitions.pxcpArtificalWait);

			endTurn(p, true);
		}
		private void handleChatMessage(ChatMessage m, ServerPlayer p)
		{
			if (m.Text == "") return; //client sent invalid request
			if (p.PlayerID != m.Id) return; //client sent invalid request
            SendAll(new ChatMessage(p.PlayerID, m.Text));
		}
		private void handleMeldMessage(MeldMessage m, ServerPlayer p)
		{
			if (!p.MyTurn)
			{
                p.Send(new ErrorMessage("It is not your turn to meld! Returning cards."));
                p.Send(new HandMessage(p.GetHand(), true));
				return; //client sent invalid request!
			}
			if (p.CompletedPhaze)
			{
                p.Send(new ErrorMessage("You have already completed your phaze! Returning cards."));
                p.Send(new HandMessage(p.GetHand(), true));
				return; //client sent invalid request!
			}
	
			List<Group> lg = m.Groups;
			
			//check that this doesn't use ALL of the player's cards!
			int num_cards = 0;
			foreach (Group g in lg)
			{
                num_cards += g.Count;
			}
            if (num_cards > this.rules.HandCards)
			{
				//invalid request!
                p.Send(new ErrorMessage("You cannot meld with ALL of the cards in your hand! You must keep one card to discard with!"));
                p.Send(new HandMessage(p.GetHand(), true));
				return;
			}
			
				
			//check that all cards are from the user's hand
            //string card_data = "";
			bool okay = true;
            bool correct = true;
			foreach (Group g in lg)
			{
				foreach (Card c in g)
				{
                    if (!p.GetHand().HasCardWithId(c.Id)) okay = false;
                    if (!g.Check()) correct = false;
                    if ((!okay) || (!correct)) break;
				}
                if ((!okay) || (!correct)) break;
			}
			if (!okay)
			{
				//invalid request
                p.Send(new ErrorMessage("You do not own any of the cards you are trying to play! Returning all cards."));
                p.Send(new HandMessage(p.GetHand(), true));
				return;
			}
            if (!correct)
            {
                //invalid request

                p.Send(new ErrorMessage("Meld is invalid! Returning all cards."));
                p.Send(new HandMessage(p.GetHand(), true));
                return;
            }
				
				
			//if we've reached here, we can officially remove all cards
			//and add the groups to the table
			foreach (Group g in lg)
			{
				foreach (Card c in g)
				{
                    p.GetHand().RemoveCardWithId(c.Id);
				}
				_table.AddGroup(g);
			}
				
			p.CompletedPhaze = true;
            p.Send(new HandMessage(p.GetHand(), false));
            SendAll(new CompletedPhazeMessage(p.PlayerID));
            SendAll(new TableMessage(_table));
			updateClientsStatuses();
		}
		#endregion
	}
}
