using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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