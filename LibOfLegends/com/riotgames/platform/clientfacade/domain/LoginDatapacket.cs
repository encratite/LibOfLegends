using System;

using FluorineFx.AMF3;

namespace com.riotgames.platform.clientfacade.domain
{
	class LoginDataPacket
	{
		public double rpBalance;
		public double minutesUntilMidnight;
		public int leaverBusterPenaltyTime;
		public bool minorShutdownEnforced;
		public bool clientHeartBeatEnabled;
		public com.riotgames.platform.statistics.PlayerStatSummaries playerStatSummaries;
		public int maxPracticeGameSize;
		public object reconnectInfo;
		public bool minor;
		public string platformId;
		public ArrayCollection gameTypeConfigs;
		public double ipBalance;
		public com.riotgames.platform.systemstate.ClientSystemStatesNotification clientSystemStates;
		public object summonerCatalog;
		public ArrayCollection languages;
		public object allSummonerData;
		public int leaverPenaltyLevel;
		public object broadcastNotification;
		public bool matchMakingEnabled;
		public bool inGhostGame;
	}
}