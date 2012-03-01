using com.riotgames.platform.gameclient.domain;
using com.riotgames.platform.summoner.spellbook;

namespace com.riotgames.platform.summoner
{
	public class AllSummonerData : AbstractDomainObject
	{
		public SpellBook spellBook;
		public SummonerDefaultSpells summonerDefaultSpells;
		public SummonerTalentsAndPoints summonerTalentsAndPoints;
		public Summoner summoner;
		public SummonerLevelAndPoints summonerLevelAndPoints;
		public SummonerLevel summonerLevel;
	}
}
