using System;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.summoner
{
	public class PublicSummoner : AbstractDomainObject
	{
		public string internalName;
		public string name;
		public int acctId;
		public int profileIconId;
		public object summonerAssociatedTalents;
		public DateTime revisionDate;
		public int revisionId;
		public int summonerLevel;
		public int summonerId;

		public PublicSummoner()
		{
		}
	}
}
