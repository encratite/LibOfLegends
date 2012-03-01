using System;

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
		//Can be null so it must not have the right type right away, otherwise it will get converted to a bogus date
		//public DateTime dodgePenaltyDate;
		public object dodgePenaltyDate;
		public string playerStatsJson;
		public PlayerStats playerStats;
	}
}