using System.Net.Sockets;
using PhazeX.Helpers;

namespace PhazeX.Network.Server
{
	
	public class RulesServer
	{
		//private int _port;
		private bool _process;
		private bool _bound;
		private Socket _listener;
		//private IPEndPoint _localEP;
		//private Message _grMsg;

		public RulesServer()
		{
			/*_port = GameSettings._NetworkSettings._RulesPort;
            _grMsg = GameRulesMessage.Construct();

			_process = true;
			_listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			_localEP = new IPEndPoint(IPAddress.Any, _port);
			try
			{
				_listener.Bind(_localEP);
				_bound = true;
			}
			catch (Exception e)
			{
				PhazeXLog.LogError(e.Message, pxFrameworkVersion.VersionString, 107);
				_bound = false;
			}*/

            _listener = null;
            _process = false;
            _bound = false;
		}

		public bool IsBound()
		{
            return true;
			//return _bound;
		}
		
		public void Unbind()
		{
			lock(this)
			{
				_process = false;
				if (_bound)
				{
                    if (_listener != null)
                    {
                        _listener.Shutdown(SocketShutdown.Both);
                    }
				}
				_bound = false;
			}
		}

        public void Stop()
        {
            lock (this)
            {
                if (_bound) Unbind();
                if (_listener != null)
                {
                    _listener.Close();
                }
                _listener = null;
            }
        }

		public void Start()
		{			
			while (_process)
			{
                /*if (_listener != null)
                {
                    if (!_listener.Poll(50, SelectMode.SelectRead))
                    {
                        Thread.Sleep(50);
                        continue;
                    }
                }*/

				//receive data
				//int available = _listener.Available;
				//byte [] buffer = new byte[available];
				//IPEndPoint tempRemoteEP = new IPEndPoint(IPAddress.Any, 0);
				//EndPoint trEP = (EndPoint)tempRemoteEP;
				//int bytesRead = _listener.ReceiveFrom(buffer, ref trEP);
				//string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

				//TODO: Send Rules back
				//send(_grMsg);
			}
			if (_listener != null) _listener.Shutdown(SocketShutdown.Both);
		}
		
		private void send(Message msg)
		{
			//if (_listener != null) _listener.Send(msg.MessageText);
		}
	}
}
