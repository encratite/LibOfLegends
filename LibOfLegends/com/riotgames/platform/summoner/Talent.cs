using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.riotgames.platform.summoner
{
	public class Talent
	{
		public int index;
		public string level5Desc;
		public int minLevel;
		public int maxRank;
		public string level4Desc;
		public int tltId;
		public string level3Desc;
		public object futuredata;
		public int talentGroupId;
		public int gameCode;
		public int minTier;
		//No idea about the type, as it is null
		public object prereqTalentGameCode;
		public int dataVersion;
		public string level2Desc;
		public string name;
		public int talentRowId;
		public string level1Desc;
	}
}
