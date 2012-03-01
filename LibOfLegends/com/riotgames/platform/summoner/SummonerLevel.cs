using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.summoner
{
	public class SummonerLevel : AbstractDomainObject
	{
		public int expTierMod;
		public int grantRp;
		public int expForLoss;
		public int summonerTier;
		public int infTierMod;
		public int expToNextLevel;
		public int expForWin;
		public int summonerLevel;
	}
}