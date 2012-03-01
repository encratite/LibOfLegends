using System;
using System.Collections.Generic;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.summoner
{
	public class Summoner : AbstractDomainObject
	{
		public string internalName;
		public int acctid;
		public bool helpFlag;
		public int sumId;
		public int profileIconId;
		public bool displayEloQuestionaire;
		public DateTime lastGameDate;
		public bool advancedTutorialFlag;
		public DateTime revisionDate;
		public int revisionId;
		public string seasonOneTier;
		public string name;
		public bool nameChangeFlag;
		public bool tutorialFlag;
		//Unknown type
		public List<object> socialNetworkUserIds;
	}
}
