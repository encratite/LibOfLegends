using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibOfLegends;

namespace LibOfLegendsExample
{
	public class ServerProfile
	{
		public string Abbreviation;
		public string LoginQueueURL;
		public string RPCURL;
	}

	public class Configuration
	{
		public AuthenticationProfile Authentication;
		public ProxyProfile Proxy;
		public List<ServerProfile> ServerProfiles;

		public Configuration()
		{
			Authentication = new AuthenticationProfile();
			Proxy = new ProxyProfile();
			ServerProfiles = new List<ServerProfile>();
		}
	}
}
