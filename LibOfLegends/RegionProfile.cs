using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOfLegends
{
	public class RegionProfile
	{
		public string LoginQueueURL;
		public string RPCURL;

		public RegionProfile(string loginQueueURL, string rpcURL)
		{
			LoginQueueURL = loginQueueURL;
			RPCURL = rpcURL;
		}
	}
}
