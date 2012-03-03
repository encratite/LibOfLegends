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

		public int TurretsDestroyed;
		public int InhibitorsDestroyed;

		public int LargestCriticalStrike;
		public int LargestMultiKill;
		public int LargestKillingSpree;

		public int TimeSpentDead;

		List<RawStat> Statistics;

		public GameResult(List<RawStat> statistics)
		{
			Statistics = statistics;

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

			TurretsDestroyed = Load("TURRETS_KILLED");
			InhibitorsDestroyed = Load("BARRACKS_KILLED");

			LargestCriticalStrike = Load("LARGEST_CRITICAL_STRIKE");
			LargestMultiKill = Load("LARGEST_MULTI_KILL");
			LargestKillingSpree = Load("LARGEST_KILLING_SPREE");

			TimeSpentDead = Load("TOTAL_TIME_SPENT_DEAD");
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

		public static List<GameResult> GetGameResults(List<PlayerGameStats> gameStats)
		{
			var output = new List<GameResult>();
			foreach(var game in gameStats)
				output.Add(new GameResult(game.statistics));
			return output;
		}
	}
}
