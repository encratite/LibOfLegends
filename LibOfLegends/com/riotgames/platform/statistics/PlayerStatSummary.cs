using System;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class PlayerStatSummary : AbstractDomainObject
	{
		public PlayerStatSummary()
		{
		}

		public int maxRating;
		public string playerStatSummaryTypeString;
		public SummaryAggStats aggregatedStats;
		public DateTime modifyDate;
		public int leaves;
		public string playerStatSummaryType;
		//May be a real value, using int out of convenience
		public int userId;
		//Always a bogus value of 0
		public int losses;
		//Always a bogus value set to either 0 or 400
		public int rating;
		public int wins;
		//One might expect a string but it's always null
		public object aggregatedStatsJson;
	}
}
