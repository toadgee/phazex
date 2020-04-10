
namespace PhazeX.Options
{
	/// <summary>
	/// Summary description for NetworkSettings.
	/// </summary>
	public class NetworkSettings
	{
		public int _RulesPort;
		public bool _Hosting;
		
		protected string _defaultHostname;
		public string _DefaultHostname
		{
			get { return _defaultHostname; }
		}
		
		protected int _defaultPort;
		public int _DefaultPort
		{
			get { return _defaultPort; }
		}

		public string _Hostname;
		
		protected int _port;
		public int _Port
		{
			get { return _port; }
			set { _port = value; }
		}

		public NetworkSettings()
		{
			_RulesPort = 8820;
			_Hosting = false;
			_defaultHostname = "127.0.0.1";
			_defaultPort = 8819;
			_Hostname = "";
			_port = -1;
		}
		
	}
}
