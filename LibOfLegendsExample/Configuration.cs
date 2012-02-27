using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibOfLegends;

namespace LibOfLegendsExample
{
	public class Configuration
	{
		public AuthenticationProfile Authentication;
		public ProxyProfile Proxy;
		public List<ServerProfile> ServerProfiles;
		public SerialisableDictionary<int, string> ChampionNames;

		public Configuration()
		{
			Authentication = new AuthenticationProfile();
			Proxy = new ProxyProfile();
			ServerProfiles = new List<ServerProfile>();
			ChampionNames = new SerialisableDictionary<int, string>();
		}
	}
}
