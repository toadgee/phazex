using System;
using System.Net.Sockets;
using System.Threading;
using PhazeX.Helpers;
using PhazeX.Network.Messages;
using PhazeX.Options;
namespace PhazeX.Network.Server
{
    /// <summary>
    /// Summary description for ServerPlayer.
    /// </summary>
    public class ServerPlayer : Player
    {
        private const string pid = "PhazeX.Network.Server.ServerPlayer";
        protected bool _listening;
        protected bool _shouldRemoveFromGame;
        protected int _invalidMessages;
        private Socket _socket = null;
        private ServerGame _serverGame = null;
        protected string _address;
        protected Hand _hand;
        protected PhazeRule _currentPhazeRule;

        private Buffer _buffer;


        public ServerPlayer(Socket sock, ServerGame sg)
            : base()
        {
            this.Connected = true;
            _listening = true;
            _shouldRemoveFromGame = false;
            _invalidMessages = 0;
            _hand = null;
            _currentPhazeRule = null;

            _socket = sock;
            _serverGame = sg;
            _address = _socket.RemoteEndPoint.ToString();

            _buffer = new Buffer();

            //start 5 seconds timer for a login message
            Thread t = new Thread(new ThreadStart(this.listenForLogin));
            t.IsBackground = true;
            t.Name = "Login listener for " + _address;
            t.Start();

            //create new login timer
            new Timer(
                new TimerCallback(this.checkForLogin)
                , null
                        , TimingDefinitions.LoginTimeout_Server
                , Timeout.Infinite);

        }
        public string GetAddress()
        {
            return _address;
        }

        public void NewHand(PhazeRule pr)
        {
            _hand = new Hand();
            _currentPhazeRule = pr;
        }
        public void AddHandCard(Card c)
        {
            _hand.Add(c);
        }
        public Hand GetHand()
        {
            return _hand;
        }

        private void listenForLogin()
        {
            try
            {
                int avail;

                //as long as we're listening and not logged in
                while ((_listening) && (!this.LoggedIn))
                {
                    //save socketAvailable locally as it may grow (but not shrink as there is only
                    //one thread removing data from _socket)

                    //if (!_socket.Poll(TimingDefinitions.ListenSocket_Server, SelectMode.SelectRead)) continue;
                    avail = _socket.Available;
                    if (avail == 0)
                    {
                        Thread.Sleep(TimingDefinitions.ListenSocket_Server);
                        continue;
                    }
                    

                    //read the data
                    _buffer.ReadFromSocket(_socket, avail);
                    _serverGame.ReceivedBytes((ulong)avail);

                    bool validated = false;

                    //while we can decode something in messages
                    while (_buffer.CanDecode())
                    {
                        //get the first decodable message and save the rest back into messages
                        byte[] b = _buffer.Decode();

                        //figure out if it's a login message
                        LoginMessage lm = null;
                        if (b[0] == (byte)pxMessages.Login)
                        {
                            try
                            {
                                //create a new login message. if it throws an exception, then
                                //we couldn't validate the login message
                                lm = new LoginMessage(b);
                                validated = true;
                            }
                            catch (BadMessageException bme)
                            {
                                PhazeXLog.LogError(bme, GameLibraryVersion.ProgramIdentifier, 0);
                                validated = false;
                            }

                            if (validated)
                            {
                                //log this into the game
                                lock (this)
                                {
                                    this.LoggedIn = true;
                                }
                                StartListeningThread();
                                lock (this)
                                {
                                    _serverGame.LoginPlayer(this, lm);
                                }
                            }
                        }

                    }
                }
            }
            catch (SocketException ex)
            {
                PhazeXLog.LogError(ex, pid, 0);
                Disconnect();
            }
            return;
        }
        private void checkForLogin(object o)
        {
            bool loggedIn;

            lock (this)
            {
                loggedIn = this.LoggedIn;
            }
            if (!loggedIn)
            {
                StopListening();
            }
            return;
        }
        public void CheckConnectivity()
        {
            if (this.Connected)
            {
                this.Send(new HeartBeatMessage());
            }
        }
        private void StartListeningThread()
        {
            Thread t = new Thread(new ThreadStart(this.Listen));
            //t.IsBackground = true;
            t.Name = "Connection from " + _address;
            t.Start();
        }
        public void Disconnect()
        {
            lock (this)
            {
                _listening = false;
                this.Connected = false;
                _shouldRemoveFromGame = true;
            }
        }
        public void InvalidMessage()
        {
            _invalidMessages++;
            if (_invalidMessages == 10) Disconnect();
        }

        public PhazeRule GetCurrentPhaze()
        {
            return _currentPhazeRule;
        }

        public void Listen()
        {
            int avail;
            byte[] b;

            try
            {
                while (_listening)
                {
                    //save socketAvailable locally as it may grow (but not shrink as there is only
                    //one thread removing data from _socket)
                    avail = _socket.Available;

                    //if (!_socket.Poll(TimingDefinitions.ListenSocket_Server, SelectMode.SelectRead)) continue;
                    if (avail == 0)
                    {
                        Thread.Sleep(TimingDefinitions.ListenSocket_Server);
                        continue;
                    }

                    //read the data
                    _buffer.ReadFromSocket(_socket, avail);
                    _serverGame.ReceivedBytes((ulong)avail);



                    //decode all messages
                    while (_buffer.CanDecode())
                    {
                        b = _buffer.Decode();
                        _serverGame.AddMessageToProcess(this, b);
                    }
                }
            }
            catch (SocketException)
            {
                Disconnect();
            }
        }


        public void RemoveFromGame()
        {
            lock (this)
            {
                _shouldRemoveFromGame = true;
            }
        }
        public void RemovedFromGame()
        {
            lock (this)
            {
                _shouldRemoveFromGame = false;
                this.Connected = false;
            }
        }

        public bool ShouldRemoveMe()
        {
            bool b;
            lock (this)
            {
                b = _shouldRemoveFromGame;
            }
            return b;
        }
        /*public bool NeedsProcessing()
        {
            return _processMe;
        }*/

        public void StopListening()
        {
            lock (this)
            {
                _listening = false;
            }
        }
        public void Send(Message msg)
        {
            lock (this)
            {
                if (!this.Connected) return;
            }
            if (msg == null) return;

            try
            {
                if (!_socket.Connected)
                {
                    Disconnect();
                }
                else
                {
                    if (msg.MessageText == null) return;
                    byte[] encodedMessage = Message.Encode(msg.MessageText);
                    _socket.Send(encodedMessage);
                    _serverGame.SentBytes((ulong)encodedMessage.Length);
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
            return;
        }
    }
}
