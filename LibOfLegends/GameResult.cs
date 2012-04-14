using System;
using System.Collections.Generic;

using com.riotgames.platform.statistics;
using com.riotgames.platform.gameclient.domain;

namespace LibOfLegends
{
	public class GameResult
	{
		public readonly bool Win;

		public readonly int Level;

		public readonly int Kills;
		public readonly int Deaths;
		public readonly int Assists;

		public readonly int GoldEarned;

		public readonly int MinionsKilled;

		public readonly int[] Items;

		public readonly int TotalDamageDealt;
		public readonly int PhysicalDamageDealt;
		public readonly int MagicalDamageDealt;

		public readonly int TotalDamageTaken;
		public readonly int PhysicalDamageTaken;
		public readonly int MagicalDamageTaken;

		public readonly int TotalHealingDone;

		public readonly int LargestCriticalStrike;
		public readonly int LargestMultiKill;
		public readonly int LargestKillingSpree;

		public readonly int TimeSpentDead;

		//Summoner's Rift and Twisted Treeline specific

		public readonly int? TurretsDestroyed;
		public readonly int? InhibitorsDestroyed;
		public readonly int? NeutralMinionsKilled;

		//Dominion specific

		public readonly int? NodesNeutralised;
		public readonly int? NodeNeutralisationAssists;
		public readonly int? NodesCaptured;

		public readonly int? VictoryPoints;
		public readonly int? Objectives;

		public readonly int? TotalScore;
		public readonly int? ObjectiveScore;
		public readonly int? CombatScore;

		public readonly int? Rank;

		List<RawStat> Statistics;
		bool IsDominion;

		public GameResult(PlayerGameStats gameStatistics)
		{
			Statistics = gameStatistics.statistics;
			IsDominion = gameStatistics.gameMapId == 8;

			int unused = 0;
			Win = Load("WIN", ref unused);

			Level = Load("LEVEL");

			Kills = Load("CHAMPIONS_KILLED");
			Deaths = Load("NUM_DEATHS");
			Assists = Load("ASSISTS");

			GoldEarned = Load("GOLD_EARNED");

			MinionsKilled = Load("MINIONS_KILLED");

			Items = new int[6];
			for (int i = 0; i < Items.Length; i++)
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

			//Summoner's Rift and Twisted Treeline specific

			NeutralMinionsKilled = MaybeLoad("NEUTRAL_MINIONS_KILLED", false);

			TurretsDestroyed = MaybeLoad("TURRETS_KILLED", false);
			InhibitorsDestroyed = MaybeLoad("BARRACKS_KILLED", false);

			//Dominion specific

			NodesNeutralised = MaybeLoad("NODE_NEUTRALIZE", true);
			NodeNeutralisationAssists = MaybeLoad("NODE_NEUTRALIZE_ASSIST", true);
			NodesCaptured = MaybeLoad("NODE_CAPTURE", true);

			VictoryPoints = MaybeLoad("VICTORY_POINT_TOTAL", true);
			Objectives = MaybeLoad("TEAM_OBJECTIVE", true);

			TotalScore = MaybeLoad("TOTAL_PLAYER_SCORE", true);
			ObjectiveScore = MaybeLoad("OBJECTIVE_PLAYER_SCORE", true);
			CombatScore = MaybeLoad("COMBAT_PLAYER_SCORE", true);

			Rank = MaybeLoad("TOTAL_SCORE_RANK", true);
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
			if (!Load(name, ref output))
			{
				//I think that some values are left undefined occasionally
				//For example, if no critical strike occurred, no information about it is stored in the associative array retrieved from the server
				//We are going to have these values default to 0 instead as it makes running queries on the database more convenient
				return 0;
			}
			return output;
		}


		//Using the boolean argument we can have values that are relevant for the game mode default to 0 instead of null when they are undefined
		int? MaybeLoad(string name, bool isDominionValue)
		{
			int output = 0;
			if (!Load(name, ref output))
			{
				if (IsDominion == isDominionValue)
					return 0;
				else
					return null;
			}
			return output;
		}
	}
}
