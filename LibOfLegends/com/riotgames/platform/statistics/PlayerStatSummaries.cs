using System;
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
		public int userId;
	}
}
