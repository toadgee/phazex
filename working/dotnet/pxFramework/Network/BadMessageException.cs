
namespace PhazeX.Network
{
	public class BadMessageException : System.Exception
	{
		private string _expectedValue;
		private string _actualValue;

		public string ExpectedValue { get { return _expectedValue; } }
		public string ActualValue { get { return _actualValue; } }

		public BadMessageException(string s) : base(s)
		{
			_expectedValue = null;
			_actualValue = null;
		}
		public BadMessageException(string s, string expected) : base(s)
		{
			_expectedValue = expected;
			_actualValue = null;
		}
		public BadMessageException(string s, string expected, string actual) : base(s)
		{
			_expectedValue = expected;
			_actualValue = actual;
		}
	}
}
