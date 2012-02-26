using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.riotgames.platform.summoner
{
	public class SummonerTalentsAndPoints
	{
		public int talentPoints;
		public int unusedTalentPoints;
		public int dataVersion;
		public List<SummonerAssociatedTalent> summonerAssociatedTalents;
		public DateTime modifyDate;
		public DateTime createDate;
		public int summonerId;
		public object futureData;
	}
}