namespace LibOfLegends
{
	public class RegionProfile
	{
		public string LoginQueueURL;
		public string RPCURL;

		public RegionProfile()
		{
		}

		public RegionProfile(string loginQueueURL, string rpcURL)
		{
			LoginQueueURL = loginQueueURL;
			RPCURL = rpcURL;
		}
	}
}
