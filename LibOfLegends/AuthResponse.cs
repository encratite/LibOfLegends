namespace LibOfLegends
{
	/// <summary>
	/// A class encapsulating the expected fields returned by the RESTful login queue.
	/// As this is deserialized from a JSON source, it should be fairly robust to
	/// additional fields.
	/// </summary>
	public class AuthResponse
	{
		public int Rate;
		public string Token;
		public string Reason;
		public string Status;
		public int Delay;
		public string User;
	}
}
