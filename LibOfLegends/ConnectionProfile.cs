namespace LibOfLegends
{
	public class ConnectionProfile
	{
		public readonly AuthenticationProfile Authentication;
		public readonly ProxyProfile Proxy;
		public readonly RegionProfile Region;

		public readonly string User;
		public readonly string Password;

		public ConnectionProfile(AuthenticationProfile authentication, RegionProfile region, ProxyProfile proxy, string user, string password)
		{
			Authentication = authentication;
			Region = region;
			Proxy = proxy;

			User = user;
			Password = password;
		}
	}
}
