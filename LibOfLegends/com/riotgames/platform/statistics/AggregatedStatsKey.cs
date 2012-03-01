using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class AggregatedStatsKey : AbstractDomainObject
	{
		public string gameMode;
		public int userId;
		public string gameModeString;
	}
}