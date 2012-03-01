using System.Collections.Generic;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class PlayerStatSummaries : AbstractDomainObject
	{
		public PlayerStatSummaries()
		{
		}

		public List<PlayerStatSummary> playerStatSummarySet;
		//May be a real value, using int out of convenience
		public int userId;
	}
}
