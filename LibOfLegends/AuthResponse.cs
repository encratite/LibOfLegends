namespace LibOfLegends
{
	/// <summary>
	/// A class encapsulating the expected fields returned by the RESTful login queue.
	/// As this is deserialized from a JSON source, it should be fairly robust to
	/// additional fields.
	/// </summary>
	public class AuthResponse
	{
		public int Rate { get; set; }
		public string Token { get; set; }
		public string Reason { get; set; }
		public string Status { get; set; }
		public int Delay { get; set; }
		public string User { get; set; }
	}
}
