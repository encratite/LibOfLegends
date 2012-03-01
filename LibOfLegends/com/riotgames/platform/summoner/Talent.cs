using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.summoner
{
	public class Talent : AbstractDomainObject
	{
		public int index;
		public string level5Desc;
		public int minLevel;
		public int maxRank;
		public string level4Desc;
		public int tltId;
		public string level3Desc;
		public int talentGroupId;
		public int gameCode;
		public int minTier;
		//No idea about the type, as it is null
		public object prereqTalentGameCode;
		public string level2Desc;
		public string name;
		public int talentRowId;
		public string level1Desc;
	}
}
