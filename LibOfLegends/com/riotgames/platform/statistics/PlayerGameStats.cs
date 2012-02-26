using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.riotgames.platform.gameclient.domain;

namespace com.riotgames.platform.statistics
{
	public class PlayerGameStats
	{
		public string skinName;
		public bool ranked;
		public int skinIndex;
		public List<FellowPlayerInfo> fellowPlayers;
		public string gameType;
		public int experienceEarned;
		public string rawStatsJson;
		public bool eligibleFirstWinOfDay;
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
		public int id;
		public int boostXpEarned;
		public int level;
		public bool invalid;
		public int dataVersion;
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
		public object futureData;
		public string gameMode;
		public string difficultyString;
		public int KCoefficient;
		public int teamRating;
		public string subType;
		public string queueType;
		public bool premadeTeam;
		public float predictedWinPct;
		public int rating;
		public int championId;
	}
}
