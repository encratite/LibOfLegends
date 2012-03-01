using System.Collections.Generic;

using LibOfLegends;
using Nil;

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
