using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class AggregatedStat : AbstractDomainObject
	{
		public string statType;
		public int count;
		public int value;
		public int championId;
	}
}