using System;

using FluorineFx.AMF3;

using com.riotgames.platform.gameclient.domain;


namespace com.riotgames.platform.statistics
{
	class PlayerStatSummaries : AbstractDomainObject
	{
		public PlayerStatSummaries()
		{
		}

		public ArrayCollection playerStatSummarySet;
		public double userId;
	}

	class PlayerStatSummary : com.riotgames.platform.gameclient.domain.AbstractDomainObject
	{
		public PlayerStatSummary()
		{
		}

		public double userId;
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

	class SummaryAggStats : com.riotgames.platform.gameclient.domain.AbstractDomainObject
	{
		public SummaryAggStats()
		{
		}

		public ArrayCollection stats;
		public object statsJson;
	}

	class SummaryAggStat : com.riotgames.platform.gameclient.domain.AbstractDomainObject
	{
		public SummaryAggStat()
		{
		}

		public string statType;
		public double value;
	}
}