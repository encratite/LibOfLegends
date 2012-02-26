using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.riotgames.platform.summoner
{
	public class SummonerAssociatedTalent
	{
		public int rank;
		public SummonerAssociatedTalentPK comp_id;
		public int dataVersion;
		public Talent talent;
		public object futureData;
	}
}
