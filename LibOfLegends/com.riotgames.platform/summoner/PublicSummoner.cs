using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.riotgames.platform.summoner
{
	public class PublicSummoner
	{
		public string internalName;
		public int dataVersion;
		public int acctId;
		public int profileIconId;
		public object summonerAssociatedTalents;
		public DateTime revisionDate;
		public int revisionId;
		public int summonerLevel;
		public int summonerId;
		public object futureData;

		public PublicSummoner()
		{
		}
	}
}
