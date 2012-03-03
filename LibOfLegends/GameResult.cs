using System;
using System.Collections.Generic;

using com.riotgames.platform.statistics;
using com.riotgames.platform.gameclient.domain;

namespace LibOfLegends
{
	public class GameResult
	{
		public bool Win;

		public int Level;

		public int Kills;
		public int Deaths;
		public int Assists;

		public int GoldEarned;

		public int MinionsKilled;
		public int NeutralMinionsKilled;

		public int[] Items;

		public int TotalDamageDealt;
		public int PhysicalDamageDealt;
		public int MagicalDamageDealt;

		public int TotalDamageTaken;
		public int PhysicalDamageTaken;
		public int MagicalDamageTaken;

		public int TotalHealingDone;

		public int LargestCriticalStrike;
		public int LargestMultiKill;
		public int LargestKillingSpree;

		public int TimeSpentDead;

		//Summoner's Rift and Twisted Treeline specific

		public int? TurretsDestroyed;
		public int? InhibitorsDestroyed;

		//Dominion specific

		public int? NodesNeutralised;
		public int? NodeNeutralisationAssists;
		public int? NodesCaptured;

		public int? VictoryPoints;
		public int? Objectives;

		public int? TotalScore;
		public int? ObjectiveScore;
		public int? CombatScore;

		public int? Rank;

		List<RawStat> Statistics;

		public GameResult(PlayerGameStats gameStatistics)
		{
			Statistics = gameStatistics.statistics;

			int unused = 0;
			Win = Load("WIN", ref unused);

			Level = Load("LEVEL");

			Kills = Load("CHAMPIONS_KILLED");
			Deaths = Load("NUM_DEATHS");
			Assists = Load("ASSISTS");

			GoldEarned = Load("GOLD_EARNED");

			MinionsKilled = Load("MINIONS_KILLED");
			NeutralMinionsKilled = Load("NEUTRAL_MINIONS_KILLED");

			Items = new int[6];
			for(int i = 0; i < Items.Length; i++)
				Items[i] = Load(string.Format("ITEM{0}", i));

			TotalDamageDealt = Load("TOTAL_DAMAGE_DEALT");
			PhysicalDamageDealt = Load("PHYSICAL_DAMAGE_DEALT_PLAYER");
			MagicalDamageDealt = Load("MAGIC_DAMAGE_DEALT_PLAYER");

			TotalDamageTaken = Load("TOTAL_DAMAGE_TAKEN");
			PhysicalDamageTaken = Load("PHYSICAL_DAMAGE_TAKEN");
			MagicalDamageTaken = Load("MAGIC_DAMAGE_TAKEN");

			TotalHealingDone = Load("TOTAL_HEAL");

			LargestCriticalStrike = Load("LARGEST_CRITICAL_STRIKE");
			LargestMultiKill = Load("LARGEST_MULTI_KILL");
			LargestKillingSpree = Load("LARGEST_KILLING_SPREE");

			TimeSpentDead = Load("TOTAL_TIME_SPENT_DEAD");

			TurretsDestroyed = MaybeLoad("TURRETS_KILLED");
			InhibitorsDestroyed = MaybeLoad("BARRACKS_KILLED");

			NodesNeutralised = MaybeLoad("NODE_NEUTRALIZE");
			NodeNeutralisationAssists = MaybeLoad("NODE_NEUTRALIZE_ASSIST");
			NodesNeutralised = MaybeLoad("NODE_CAPTURE");

			VictoryPoints = MaybeLoad("VICTORY_POINT_TOTAL");
			Objectives = MaybeLoad("TEAM_OBJECTIVE");

			TotalScore = MaybeLoad("TOTAL_PLAYER_SCORE");
			ObjectiveScore = MaybeLoad("OBJECTIVE_PLAYER_SCORE");
			CombatScore = MaybeLoad("COMBAT_PLAYER_SCORE");

			Rank = MaybeLoad("TOTAL_SCORE_RANK");
		}

		bool Load(string name, ref int output)
		{
			foreach (var stat in Statistics)
			{
				if (stat.statType == name)
				{
					output = stat.value;
					return true;
				}
			}
			return false;
		}

		int Load(string name)
		{
			int output = 0;
			if(!Load(name, ref output))
				throw new Exception("Unable to find stat " + name);
			return output;
		}

		int? MaybeLoad(string name)
		{
			int output = 0;
			if (!Load(name, ref output))
				return null;
			return output;
		}
	}
}
