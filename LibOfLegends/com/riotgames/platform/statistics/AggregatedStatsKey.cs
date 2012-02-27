using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class AggregatedStatsKey : AbstractDomainObject
	{
		public string gameMode;
		public int userId;
		public string gameModeString;
	}
}