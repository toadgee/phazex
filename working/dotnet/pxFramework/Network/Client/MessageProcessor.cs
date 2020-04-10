using System;
using System.Collections;
using PhazeX.Network;
using PhazeX.Network.Client;
using PhazeX;
using PhazeX.Options;
using PhazeX.Network.Messages;
using System.Collections.Generic;

namespace PhazeX.Network.Client
{
	public class MessageProcessor
	{
		public Client _Client = null;

		#region Bad Message
		public delegate void BadMessageDelegate(BadMessageException bme);
		public BadMessageDelegate _BadMessageDelegate = null;
		public void ProcessBadMessage(BadMessageException bme)
		{
			if (_Client == null) return;
			if (_BadMessageDelegate != null)
			{
				_BadMessageDelegate(bme);
			}
		}

		#endregion
		#region Change Name Message
		public ChangeNameMessage.ChangeNameMessageDelegate _ChangeNameMessageDelegate = null;
		public void ProcessChangeNameMessage(ChangeNameMessage cnm)
		{
            throw new NotImplementedException();
            /*
			if (_Client == null) return;
			Player p = _Client.FindPlayer(cnm.PlayerId, true);
			lock(typeof(Player))
			{
				p.Name = cnm.NewName;
			}
			lock(typeof(Client))
			{
				if (cnm.PlayerId == _Client._PlayerID)
				{
					GameSettings.PlayerSettings._Name = cnm.NewName;
				}
			}

			if (_ChangeNameMessageDelegate != null)
			{
                _ChangeNameMessageDelegate(cnm.OldName, cnm.NewName, p);
			}
            */
		}

		#endregion
		#region Change Name Reject Message
		public ChangeNameRejectMessage.ChangeNameRejectMessageDelegate _ChangeNameRejectMessageDelegate = null;
		public void ProcessChangeNameRejectMessage(ChangeNameRejectMessage m)
		{
			if (_Client == null) return;
			if (_ChangeNameRejectMessageDelegate != null)
			{
				_ChangeNameRejectMessageDelegate();
			}
		}
		
		#endregion
		#region Chat Message
		public ChatMessage.ChatMessageDelegate _ChatMessageDelegate = null;
		public void ProcessChatMessage(ChatMessage cm)
		{
			if (_Client == null) return;
			if (_ChatMessageDelegate != null)
			{
				Player p = _Client.FindPlayer(cm.Id, true);
				_ChatMessageDelegate(p, cm.Text);
			}
		}
		
		#endregion
		#region Completed Phaze Message
        public CompletedPhazeMessage.CompletedPhazeMessageDelegate _CompletedPhazeMessageDelegate = null;
		public void ProcessCompletedPhazeMessage(CompletedPhazeMessage m)
		{
			if (_Client == null) return;
			Player p = _Client.FindPlayer(m.PlayerId, true);

            if (p.PlayerID == _Client._PlayerID)
            {
                lock (typeof(Client))
                {
                    _Client._MadePhaze = true;
                }
            }

			lock(typeof(Player))
			{
				p.CompletedPhaze = true;
			}
			if (_CompletedPhazeMessageDelegate != null)
			{
				_CompletedPhazeMessageDelegate(p);
			}
		}
		
		#endregion
		#region Current Phaze Message
        public CurrentPhazeMessage.CurrentPhazeMessageDelegate _CurrentPhazeMessageDelegate = null;
		public void ProcessCurrentPhazeMessage(CurrentPhazeMessage m)
		{
			if (_Client == null) return;
            int num = m.CurrentPhaze;
			lock(typeof(Client))
			{
				_Client._CurrentPhazeRuleNumber = num;
			}
			
			if (_CurrentPhazeMessageDelegate != null)
			{
				_CurrentPhazeMessageDelegate(num);
			}
		}
		
		#endregion
		#region Dialog Message
		public DialogMessage.DialogMessageDelegate _DialogMessageDelegate = null;
		public void ProcessDialogMessage(DialogMessage m)
		{
			if (_Client == null) return;
			if (_DialogMessageDelegate != null)
			{
				_DialogMessageDelegate(m.Text);
			}
		}
		
		#endregion
		#region Discard Skip Message
		public DiscardSkipMessage.DiscardSkipMessageDelegate _DiscardSkipMessageDelegate = null;
		public void ProcessDiscardSkipMessage(DiscardSkipMessage m)
		{
			if (_Client == null) return;
			if (_DiscardSkipMessageDelegate != null)
			{
				Player p = _Client.FindPlayer(m.PlayerId, true);
				_DiscardSkipMessageDelegate(p);
			}
		}
		
		#endregion
		#region Error Message
		public ErrorMessage.ErrorMessageDelegate _ErrorMessageDelegate = null;
		public void ProcessErrorMessage(ErrorMessage m)
		{
			if (_Client == null) return;
			if (_ErrorMessageDelegate != null)
			{
				_ErrorMessageDelegate(m.Text);
			}
		}

		#endregion
		#region Game Over Message
		public GameOverMessage.GameOverMessageDelegate _GameOverMessageDelegate = null;
		public void ProcessGameOverMessage(GameOverMessage m)
		{
			if (_Client == null) return;
			if (_GameOverMessageDelegate != null)
			{
				_GameOverMessageDelegate();
			}
		}
		
		#endregion
		#region Game Rules Message
		public GameRulesMessage.GameRulesMessageDelegate _GameRulesMessageDelegate = null;
		public void ProcessGameRulesMessage(GameRulesMessage m)
		{
			if (_Client == null) return;
			if (_GameRulesMessageDelegate != null)
			{
				_GameRulesMessageDelegate();
			}
		}

		#endregion
		#region Game Starting Message
		public GameStartingMessage.GameStartingMessageDelegate _GameStartingMessageDelegate = null;
		public void ProcessGameStartingMessage(GameStartingMessage m)
		{
			if (_Client == null) return;
			lock(typeof(Client))
			{
                int max = _Client.GetPlayerCount();
                for (int ctr = 0; ctr < max; ctr++)
                {
                    Player p = _Client.GetPlayerAtPos(ctr);
                    p.ReadyToPlay = false;
                }
				_Client._GameStarted = true;
			}
			if (_GameStartingMessageDelegate != null)
			{
				_GameStartingMessageDelegate();
			}
		}

		#endregion
		#region Goodbye Message
		public GoodbyeMessage.GoodbyeMessageDelegate _GoodbyeMessageDelegate = null;
		public void ProcessGoodbyeMessage(GoodbyeMessage gm)
		{
			if (_Client == null) return;
			_Client.Disconnect();
			if (_GoodbyeMessageDelegate != null)
			{
				_GoodbyeMessageDelegate();
			}
		}

		#endregion
		#region Got Cards Message
		public GotCardsMessage.GotCardsMessageDelegate _GotCardsMessageDelegate = null;
		public void ProcessGotCardsMessage(GotCardsMessage m)
		{
			if (_Client == null) return;

            lock (typeof(Client))
            {
                foreach (Card c in m.Cards)
                {
                    _Client._Hand.Add(c);
                }
            }

			if (_GotCardsMessageDelegate != null)
			{
				_GotCardsMessageDelegate(m.Cards);
			}
		}

		#endregion
		#region Got Deck Card Message
		public GotDeckCardMessage.GotDeckCardMessageDelegate _GotDeckCardMessageDelegate = null;
		public void ProcessGotDeckCardMessage(GotDeckCardMessage m)
		{
			if (_Client == null) return;
			if (_GotDeckCardMessageDelegate != null)
			{
				Player p = _Client.FindPlayer(m.PlayerId, true);			
				_GotDeckCardMessageDelegate(p);
			}
		}

		#endregion
		#region Got Discard Message
		public GotDiscardMessage.GotDiscardMessageDelegate _GotDiscardMessageDelegate = null;
		public void ProcessGotDiscardMessage(GotDiscardMessage m)
		{
			if (_Client == null) return;			
			if (_GotDiscardMessageDelegate != null)
			{
				Player p = _Client.FindPlayer(m.PlayerId, true);
                _GotDiscardMessageDelegate(p, m.CardObj);
			}
		}

		#endregion
		#region Hand Message
		public HandMessage.HandMessageDelegate _HandMessageDelegate = null;
		public void ProcessHandMessage(HandMessage m)
		{
			if (_Client == null) return;
			int hand_cards = _Client.GameRules.HandCards;
			if (_Client._MyTurn) hand_cards += 1;
			
			lock(typeof(Client))
			{
				//create new hand object
                if ((m.HandObj == null) || (_Client._Hand == null) || (m.Replacement))
				{
                    _Client._Hand = m.HandObj;
				}
				else
				{
					//determine which cards to remove (are no longer in the new hand)
					List<Card> remove = new List<Card>();
					foreach (Card c in _Client._Hand)
					{
                        if (!m.HandObj.HasCardWithId(c.Id))
						{
							remove.Add(c);
						}
					}
					//remove cards
                    foreach (Card cd in remove)
					{
                        _Client._Hand.RemoveCardWithId(cd.Id);
					}

					//determine which cards to add
					List<Card> add = new List<Card>();
					foreach (Card c in m.HandObj)
					{
                        if (!_Client._Hand.HasCardWithId(c.Id))
						{
							add.Add(c);
						}
					}
					//add cards
					foreach (Card cd in add)
					{
						_Client._Hand.Add(cd);
					}

				}
			}
			if (_HandMessageDelegate != null)
			{
				_HandMessageDelegate(m.Replacement);
			}
		}
		#endregion
		#region Heartbeat Message
		public HeartBeatMessage.HeartbeatMessageDelegate _HeartbeatMessageDelegate = null;
		public void ProcessHeartbeatMessage(HeartBeatMessage hbm)
		{
			if (_Client == null) return;
			if (_HeartbeatMessageDelegate != null)
			{
				_HeartbeatMessageDelegate();
			}
		}
		#endregion
		#region Login Message
		public LoginMessage.LoginMessageDelegate _LoginMessageDelegate = null;
		public void ProcessLoginMessage(LoginMessage lm)
		{
			if (_Client == null) return;
			Player p = new Player(lm.ComputerPlayer);
            p.PlayerID = lm.Id;
			p.Name = lm.Username;
            p.Release = lm.Major;
            p.Revision = lm.Minor;
            p.Build = lm.Build;
            p.LibRelease = lm.Lib_Major;
            p.LibRevision = lm.Lib_Minor;
			p.LibBuild = lm.Lib_Build;
			p.ClientTitle = lm.Title;
            p.FrameworkVersion = lm.Framework;
			p.OSVersion = lm.Os;
			if (lm.Ready) p.ReadyToPlay = true;
			
			_Client.AddPlayer(p);
			if (_LoginMessageDelegate != null)
			{
				_LoginMessageDelegate(p, lm.Fresh);
			}
		}
		#endregion
		#region Logoff Message
		public LogOffMessage.LogOffMessageDelegate _LogOffMessageDelegate = null;
		public void ProcessLogOffMessage(LogOffMessage m)
		{
			if (_Client == null) return;
			Player p = null;
			p = _Client.FindPlayer(m.Id, true);
			if (!p.Connected)
			{
				throw new BadMessageException("Logoff Message : Player is already logged off!");
			}
			lock(typeof(Player))
			{
				p.Connected = false;
			}
			if (m.Remove)
			{
				_Client.RemovePlayer(p);
			}

			if (_LogOffMessageDelegate != null)
			{
				_LogOffMessageDelegate(p);
			}
		}

		#endregion
		#region New Hand Message
		public NewHandMessage.NewHandMessageDelegate _NewHandMessageDelegate = null;
		public void ProcessNewHandMessage(NewHandMessage m)
		{
			if (_Client == null) return;
			lock(typeof(Client))
			{
				_Client._Hand = new Hand();
				_Client._MadePhaze = false;
				_Client._FirstDiscard = 2;
			}
			if (_NewHandMessageDelegate != null)
			{
				_NewHandMessageDelegate();
			}
		}
		#endregion
		#region Played Card On Table Message
		public PlayedCardOnTableMessage.PlayedCardOnTableMessageDelegate _PlayedCardOnTableMessageDelegate = null;
		public void ProcessPlayedCardOnTableMessage(PlayedCardOnTableMessage m)
		{
			if (_Client == null) return;
			if (_PlayedCardOnTableMessageDelegate != null)
			{
				Player p = _Client.FindPlayer(m.PlayerId, true);
				_PlayedCardOnTableMessageDelegate(p, m.GroupObj, m.CardObj);
			}
		}
		#endregion
		#region Ready Message
		public ReadyMessage.ReadyMessageDelegate _ReadyMessageDelegate = null;
		public void ProcessReadyMessage(ReadyMessage m)
		{
			if (_Client == null) return;
			Player p = _Client.FindPlayer(m.Id, true);
			lock(p)
			{
				p.ReadyToPlay = m.Ready;
			}
			if (_ReadyMessageDelegate != null)
			{
				_ReadyMessageDelegate(p);
			}
		}
		#endregion
		#region Scoreboard Message
		public ScoreboardMessage.ScoreboardMessageDelegate _ScoreboardMessageDelegate = null;
		public void ProcessScoreboardMessage(ScoreboardMessage m)
		{
			if (_Client == null) return;
			Player p = _Client.FindPlayer(m.PlayerId, true);
			lock(typeof(Player))
			{
				p.Scoreboard = m.ScoreboardObj;
			}
			if (_ScoreboardMessageDelegate != null)
			{
				_ScoreboardMessageDelegate(p);
			}
		}
		#endregion
		#region Skip Notification Message
		public SkipNotificationMessage.SkipNotificationMessageDelegate _SkipNotificationMessageDelegate = null;
		public void ProcessSkipNotificationMessage(SkipNotificationMessage m)
		{
			if (_Client == null) return;
			Player p = _Client.FindPlayer(m.PlayerId, true);
			lock(p)
			{
				p.Skipped();
			}
			if (_SkipNotificationMessageDelegate != null)
			{
				_SkipNotificationMessageDelegate(p);
			}
		}
		#endregion
		#region Start Game Timer
		public StartGameTimerMessage.StartGameTimerMessageDelegate _StartGameTimerMessageDelegate = null;
		public void ProcessStartGameTimerMessage(StartGameTimerMessage m)
		{
			if (_Client == null) return;
			if (_StartGameTimerMessageDelegate != null)
			{
				_StartGameTimerMessageDelegate(m.Val);
			}
		}
		#endregion
		#region Status Message
		public StatusMessage.StatusMessageDelegate _StatusMessageDelegate = null;
		public void ProcessStatusMessage(StatusMessage sm)
		{
			if (_Client == null) return;
			Player p = _Client.FindPlayer(sm.Id, true);
			lock(typeof(Player))
			{
                p.CurrentPhaze = sm.CurrentPhaze;
                p.CompletedPhaze = sm.MadePhaze;
                //TODO p.CardsInHand = sm.CardsInHand;
                p.SkipsLeft = sm.SkipsLeft;
			}
			if (_StatusMessageDelegate != null)
			{
				_StatusMessageDelegate(p);
			}
		}
		#endregion
		#region System Message
		public SystemMessage.SystemMessageDelegate _SystemMessageDelegate = null;
		public void ProcessSystemMessage(SystemMessage m)
		{
			if (_Client == null) return;
			if (_SystemMessageDelegate != null)
			{
				_SystemMessageDelegate(m.Text);
			}
		}
		#endregion
		#region Table Message
		public TableMessage.TableMessageDelegate _TableMessageDelegate = null;
		public void ProcessTableMessage(TableMessage m)
		{
			if (_Client == null) return;
			lock(_Client)
			{
                _Client._Table = m.TableObj;
			}
			if (_TableMessageDelegate != null)
			{
				_TableMessageDelegate();
			}
		}
		#endregion
		#region Turn End Message
		public TurnEndMessage.TurnEndMessageDelegate _TurnEndMessageDelegate = null;
		public void ProcessTurnEndMessage(TurnEndMessage m)
		{
			if (_Client == null) return;
			Player p = _Client.FindPlayer(m.Id, true);
			lock(typeof(Player))
			{
				p.MyTurn = false;
			}

			if (p.PlayerID == _Client._PlayerID)
			{
				lock(typeof(Client))
				{
					_Client._MyTurn = false;
				}
			}
		
			if (_TurnEndMessageDelegate != null)
			{
				_TurnEndMessageDelegate(p);
			}
		}
		#endregion
		#region Turn Start Message
        public TurnStartMessage.TurnStartMessageDelegate _TurnStartMessageDelegate = null;
		public void ProcessTurnStartMessage(TurnStartMessage m)
		{
			if (_Client == null) return;
			Player p = _Client.FindPlayer(m.Id, true);
			lock(typeof(Player))
			{
				p.MyTurn = true;
			}

			if (p.PlayerID == _Client._PlayerID)
			{
				lock(typeof(Client))
				{
					_Client._MyTurn = true;
					_Client._PickedCard = false;
				}
			}
			if (_TurnStartMessageDelegate != null)
			{
				_TurnStartMessageDelegate(p);
			}
			
		}
		#endregion
		#region Unknown Message
        public UnknownMessage.UnknownMessageDelegate _UnknownMessageDelegate = null;
		public void ProcessUnknownMessage(UnknownMessage m)
		{
			if (_Client == null) return;
			if (_UnknownMessageDelegate != null)
			{
				_UnknownMessageDelegate(m.MessageText);
			}
		}
		#endregion
		#region Update Discard Message	
		public UpdateDiscardMessage.UpdateDiscardMessageDelegate _UpdateDiscardMessageDelegate = null;
		public void ProcessUpdateDiscardMessage(UpdateDiscardMessage m)
		{
			if (_Client == null) return;

			lock(_Client)
			{
				_Client._FirstDiscard--;
				_Client._Discard = m.CardObj;
			}

			if (_UpdateDiscardMessageDelegate != null)
			{
                Player p = _Client.FindPlayer(m.PlayerId, false);
				_UpdateDiscardMessageDelegate(p, m.Discarded, m.CardObj);
			}
		}
		#endregion
		#region Went Out Message
		public WentOutMessage.WentOutMessageDelegate _WentOutMessageDelegate = null;
		public void ProcessWentOutMessage(WentOutMessage m)
		{
			if (_Client == null) return;
			Player p = _Client.FindPlayer(m.PlayerId, true);
			if (_WentOutMessageDelegate != null)
			{
				_WentOutMessageDelegate(p);
			}
		}
		#endregion
		#region Won Message
		public WonMessage.WonMessageDelegate _WonMessageDelegate = null;
		public void ProcessWonMessage(WonMessage m)
		{
			if (_Client == null) return;
            List<Player> winningPlayers = new List<Player>(m.PlayerIds.Length);
            foreach (int id in m.PlayerIds)
			{
				Player p = _Client.FindPlayer(id, true);
				winningPlayers.Add(p);
			}
			if (_WonMessageDelegate != null)
			{
				_WonMessageDelegate(winningPlayers, m.Points);
			}
		}
		#endregion
	}
}
