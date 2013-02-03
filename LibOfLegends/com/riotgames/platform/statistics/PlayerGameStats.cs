using System;
using System.Collections.Generic;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class PlayerGameStats : AbstractDomainObject
	{
		public string skinName;
		public bool ranked;
		public int skinIndex;
		public List<FellowPlayerInfo> fellowPlayers;
		public string gameType;
		public int experienceEarned;
		// Always null
		public object rawStatsJson;
		public bool eligibleFirstWinOfDay;
		// Always null
		public object difficulty;
		public int gameMapId;
		public bool leaver;
		public int spell1;
		public string gameTypeEnum;
		public int teamId;
		public int summonerId;
		public List<RawStat> statistics;
		public int spell2;
		public bool afk;
		// Is always set to null now, used to be an integer
		public object id;
		public int boostXpEarned;
		public int level;
		public bool invalid;
		public int userId;
		public DateTime createDate;
		public int userServerPing;
		public int adjustedRating;
		public int premadeSize;
		public int boostIpEarned;
		public int gameId;
		public int timeInQueue;
		public int ipEarned;
		public int eloChange;
		public string gameMode;
		// Always null
		public object difficultyString;
		// Always 0
		public int KCoefficient;
		// Always 0
		public int teamRating;
		public string subType;
		public string queueType;
		public bool premadeTeam;
		// Always 0
		public double predictedWinPct;
		// Always 0
		public int rating;
		public int championId;
	}
}
