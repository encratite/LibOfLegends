using com.riotgames.platform.summoner;
using com.riotgames.platform.summoner.spellbook;

namespace com.riotgames.platform.gameclient.domain
{
	public class AllPublicSummonerDataDTO : AbstractDomainObject
	{
		public BasePublicSummonerDTO summoner;
		public SpellBook spellBook;
		public SummonerDefaultSpells summonerDefaultSpells;
		public SummonerTalentsAndPoints summonerTalentsAndPoints;
		public SummonerLevelAndPoints summonerLevelAndPoints;
		public SummonerLevel summonerLevel;
	}
}