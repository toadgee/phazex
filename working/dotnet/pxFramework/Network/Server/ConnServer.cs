using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using PhazeX.Helpers;
using PhazeX.Options;

namespace PhazeX.Network.Server
{
	public class ConnServer
	{
		private bool _process;
		private bool _bound;
		private int _port;
		private TcpListener _connectionListener;
		private ServerGame _serverGame;
        private NetworkSettings settings;
        private GameRules rules;
		
		public ConnServer(ServerGame sg, GameRules rules, NetworkSettings settings)
		{
            this.rules = rules;
            this.settings = settings;
			_serverGame = sg;
			_port = this.settings._Port;
			_process = true;
			try
			{
				_connectionListener = new TcpListener(IPAddress.Any, _port);
				_bound = true;
			}
			catch (Exception e)
			{
				PhazeXLog.LogError(e, GameLibraryVersion.VersionString, 105);
				_bound = false;
				_process = false;
				throw e;
			}		
		}
		
		public bool IsBound()
		{
			bool i;
			lock(this)
			{
				i = _bound;
			}
			return i;
		}
		
		public void Stop()
		{
			lock(this)
			{
				_process = false;
                if (_bound)
                {
                    if (_connectionListener != null)
                    {
                        _connectionListener.Stop();
                        _connectionListener = null;
                    }
                }
                _bound = false;
			}
		}

	
		public void Start()
		{
            bool cl_started = false;
            if (_process)
            {
                try
                {
                    _connectionListener.Start();

                    //while (_connectionListener.Server == null) continue;
                    //while (!_connectionListener.Server.Poll(50, SelectMode.SelectRead)) continue;
                    cl_started = true;
                }
                catch (Exception e)
                {
                    PhazeXLog.LogError(e, GameLibraryVersion.VersionString, 106);
                    _process = false;
                }
            }
            
			while (_process)
			{
                try
                {
                    bool b = false;
                    lock(this)
                    {
                        if (_connectionListener != null) b = _connectionListener.Pending();
                    }
                    if (!b)
                    {
                        Thread.Sleep(TimingDefinitions.ConnectionListener_Pending);
                        continue;
                    }
                    Socket sck = null;
                    lock (this)
                    {
                        if (_connectionListener == null) sck = null;
                        else sck = _connectionListener.AcceptSocket();
                    }
                    if (sck == null) continue;
                    new ServerPlayer(sck, _serverGame);
                    if (_serverGame.PlayerCount() == this.rules.MaximumPlayers)
                    {
                        lock (this)
                        {
                            _process = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    PhazeXLog.LogError(e, GameLibraryVersion.VersionString, 0);
                    continue;
                }
			}
			if (cl_started) if (_connectionListener != null) _connectionListener.Stop();
		}
	}
}
