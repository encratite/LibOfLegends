using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class SummaryAggStat : AbstractDomainObject
	{
		public SummaryAggStat()
		{
		}

		public string statType;
		//It's really a real value, casting it to integer out of convenience
		//public double value;
		public int value;
	}
}
