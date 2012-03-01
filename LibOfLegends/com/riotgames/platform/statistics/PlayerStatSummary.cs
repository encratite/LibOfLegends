using System;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class PlayerStatSummary : AbstractDomainObject
	{
		public PlayerStatSummary()
		{
		}

		//May be a real value, using int out of convenience
		public int userId;
		public int maxRating;
		public int leaves;
		public DateTime modifyDate;
		public int losses;
		public int rating;
		public int wins;
		public string playerStatSummaryType;
		public string playerStatSummaryTypeString;
		public SummaryAggStats aggregatedStats;
		public object aggregatedStatsJson;
	}
}
