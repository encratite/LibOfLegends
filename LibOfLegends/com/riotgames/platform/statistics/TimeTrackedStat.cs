using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class TimeTrackedStat : AbstractDomainObject
	{
		public DateTime timestamp;
		public string type;
	}
}