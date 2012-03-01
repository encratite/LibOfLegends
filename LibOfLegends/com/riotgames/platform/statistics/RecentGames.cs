using System.Collections.Generic;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class RecentGames : AbstractDomainObject
	{
		public string recentGamesJson;
		public List<PlayerGameStats> gameStatistics;
		public int userId;

		public RecentGames()
		{
		}
	}
}
