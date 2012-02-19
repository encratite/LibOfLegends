using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.riotgames.platform.statistics
{
	public class RecentGames
	{
		public string recentGamesJson;
		public List<PlayerGameStats> gameStatistics;
		public int dataVersion;
		public int userId;
		public object futureData;

		public RecentGames()
		{
		}
	}
}
