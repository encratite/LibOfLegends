using System;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class TimeTrackedStat : AbstractDomainObject
	{
		public DateTime timestamp;
		public string type;
	}
}