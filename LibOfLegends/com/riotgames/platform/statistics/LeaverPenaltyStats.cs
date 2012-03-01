using System;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class LeaverPenaltyStats : AbstractDomainObject
	{
		//Unknown type
		public object lastLevelIncrease;
		public int level;
		public DateTime lastDecay;
		public bool userInformed;
		public int points;
	}
}