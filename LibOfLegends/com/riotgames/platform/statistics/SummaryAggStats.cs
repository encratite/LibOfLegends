using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class SummaryAggStats : AbstractDomainObject
	{
		public SummaryAggStats()
		{
		}

		public List<SummaryAggStat> stats;
		public string statsJson;
	}
}
