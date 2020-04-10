
namespace PhazeX.Options
{
	/// <summary>
	/// Summary description for PlayerSettings.
	/// </summary>
	public class PlayerSettings
	{
		private string _name;
		public string _Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public PlayerSettings()
		{
			_name = "username";
		}
	}
}
