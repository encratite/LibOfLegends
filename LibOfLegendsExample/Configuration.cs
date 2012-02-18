using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		public List<ServerProfile> ServerProfiles;

		public Configuration()
		{
			ServerProfiles = new List<ServerProfile>();
		}
	}
}
