using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class PlayerStats : AbstractDomainObject
	{
		public List<TimeTrackedStat> timeTrackedStats;
		public int promoGamesPlayed;
		//always null?
		public object promoGamesPlayedLastUpdated;
	}
}