using System.Collections.Generic;

namespace com.riotgames.platform.login
{
	class Session
	{
		public Session()
		{
			accountSummary = new Dictionary<string, object>();
		}

		public string token { get; set; }
		public string password { get; set; }
		public Dictionary<string, object> accountSummary { get; set; }
	}
}