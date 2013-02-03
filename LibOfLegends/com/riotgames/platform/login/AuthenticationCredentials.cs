using System.Collections.Generic;

namespace com.riotgames.platform.login
{
	class AuthenticationCredentials
	{
		public AuthenticationCredentials()
		{
		}

		public string partnerCredentials { get; set; }
		public string oldPassword { get; set; }
		public string domain { get; set; }
		public string ipAddress { get; set; }
		public string authToken { get; set; }
		public string locale { get; set; }
		public string clientVersion { get; set; }
		public string password { get; set; }
		public string username { get; set; }
		public string securityAnswer { get; set; }
	}
}
