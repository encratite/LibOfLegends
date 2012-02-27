using System;
using System.Collections.Generic;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class PlayerLifeTimeStats : AbstractDomainObject
	{
		public PlayerStatSummaries playerStatSummaries;
		public LeaverPenaltyStats leaverPenaltyStats;
		public DateTime previousFirstWinOfDay;
		public int userId;
		public int dodgeStreak;
		public int dodgePenaltyDate;
		public string playerStatsJson;
		public PlayerStats playerStats;
	}
}