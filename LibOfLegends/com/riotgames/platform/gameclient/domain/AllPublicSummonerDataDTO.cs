using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.riotgames.platform.summoner;
using com.riotgames.platform.summoner.spellbook;

namespace com.riotgames.platform.gameclient.domain
{
	public class AllPublicSummonerDataDTO
	{
		public int dataVersion;
		public BasePublicSummonerDTO summoner;
		public SpellBook spellBook;
		public SummonerDefaultSpells summonerDefaultSpells;
		public SummonerTalentsAndPoints summonerTalentsAndPoints;
		public SummonerLevelAndPoints summonerLevelAndPoints;
		public SummonerLevel summonerLevel;
		public object futureData;
	}
}