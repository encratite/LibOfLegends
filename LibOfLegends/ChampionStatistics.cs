using System;
using System.Collections.Generic;
using System.Linq;

using com.riotgames.platform.statistics;

namespace LibOfLegends
{
	public class ChampionStatistics
	{
		public readonly int Wins;
		public readonly int Losses;

		public readonly int Kills;
		public readonly int Deaths;
		public readonly int Assists;

		public readonly int MinionKills;

		public readonly int Gold;

		public readonly int TurretsDestroyed;

		public readonly int DamageDealt;
		public readonly int PhysicalDamageDealt;
		public readonly int MagicalDamageDealt;

		public readonly int DamageTaken;

		public readonly int DoubleKills;
		public readonly int TripleKills;
		public readonly int QuadraKills;
		public readonly int PentaKills;

		public readonly int TimeSpentDead;

		public readonly int MaximumKills;
		public readonly int MaximumDeaths;

		public readonly int ChampionId;

		AggregatedStats Statistics;

		//Must be set by user
		public string Name;

		public ChampionStatistics(int championId, AggregatedStats statistics)
		{
			ChampionId = championId;
			Statistics = statistics;

			Wins = Load("TOTAL_SESSIONS_WON");
			Losses = Load("TOTAL_SESSIONS_LOST");

			Kills = Load("TOTAL_CHAMPION_KILLS");
			Deaths = Load("TOTAL_DEATHS_PER_SESSION");
			Assists = Load("TOTAL_ASSISTS");

			MinionKills = Load("TOTAL_MINION_KILLS");

			Gold = Load("TOTAL_GOLD_EARNED");

			TurretsDestroyed = Load("TOTAL_TURRETS_KILLED");

			DamageDealt = Load("TOTAL_DAMAGE_DEALT");
			PhysicalDamageDealt = Load("TOTAL_PHYSICAL_DAMAGE_DEALT");
			MagicalDamageDealt = Load("TOTAL_MAGIC_DAMAGE_DEALT");

			DamageTaken = Load("TOTAL_DAMAGE_TAKEN");

			DoubleKills = Load("TOTAL_DOUBLE_KILLS");
			TripleKills = Load("TOTAL_TRIPLE_KILLS");
			QuadraKills = Load("TOTAL_QUADRA_KILLS");
			PentaKills = Load("TOTAL_PENTA_KILLS");

			TimeSpentDead = Load("TOTAL_TIME_SPENT_DEAD");

			MaximumKills = Load("MAX_CHAMPIONS_KILLED");
			MaximumDeaths = Load("MAX_NUM_DEATHS");
		}

		int Load(string name)
		{
			foreach(var stat in Statistics.lifetimeStatistics)
			{
				if (stat.championId == ChampionId && stat.statType == name)
					return stat.value;
			}
			throw new Exception("Unable to find stat " + name + " for champion " + ChampionId);
		}

		public int Games()
		{
			return Wins + Losses;
		}

		public double WinRatio()
		{
			return (double)Wins / Games();
		}

		public double KillsPerGame()
		{
			return (double)Kills / Games();
		}

		public double DeathsPerGame()
		{
			return (double)Deaths / Games();
		}

		public double AssistsPerGame()
		{
			return (double)Assists / Games();
		}

		public double KillsPerDeath()
		{
			return (double)Kills / Deaths;
		}

		public double KillsAndAssistsPerDeath()
		{
			return (double)(Kills + Assists) / Deaths;
		}

		public static List<ChampionStatistics> GetChampionStatistics(AggregatedStats statistics)
		{
			Dictionary<int, ChampionStatistics> output = new Dictionary<int, ChampionStatistics>();
			foreach (var statisticsEntry in statistics.lifetimeStatistics)
			{
				int key = statisticsEntry.championId;
				if (key == 0)
					continue;
				if (output.ContainsKey(key))
					continue;
				ChampionStatistics newEntry = new ChampionStatistics(key, statistics);
				output[key] = newEntry;
			}
			return output.Values.ToList();
		}
	}
}
