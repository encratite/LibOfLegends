using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOfLegends
{
	public class ConnectionProfile
	{
		public AuthenticationProfile Authentication;
		public ProxyProfile Proxy;
		public RegionProfile Region;

		public string User;
		public string Password;

		public ConnectionProfile(AuthenticationProfile authentication, RegionProfile region, ProxyProfile proxy, string user, string password)
		{
			Authentication = authentication;
			Region = region;
			User = user;
			Password = password;
			Proxy = proxy;
		}
	}
}
