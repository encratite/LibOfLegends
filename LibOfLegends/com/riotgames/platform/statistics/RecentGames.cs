using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
