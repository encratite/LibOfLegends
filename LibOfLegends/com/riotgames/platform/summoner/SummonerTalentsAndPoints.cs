using System;
using System.Collections.Generic;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.summoner
{
	public class SummonerTalentsAndPoints : AbstractDomainObject
	{
		public int talentPoints;
		public int unusedTalentPoints;
		public List<SummonerAssociatedTalent> summonerAssociatedTalents;
		public DateTime modifyDate;
		public DateTime createDate;
		public int summonerId;
	}
}