using System;
using System.Collections.Generic;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class AggregatedStats : AbstractDomainObject
	{
		public List<AggregatedStat> lifetimeStatistics;
		public DateTime modifyDate;
		public AggregatedStatsKey key;
		public string aggregatedStatsJson;
	}
}