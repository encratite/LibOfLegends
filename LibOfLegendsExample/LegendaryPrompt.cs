using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using LibOfLegends;

using com.riotgames.platform.statistics;
using com.riotgames.platform.summoner;

namespace LibOfLegendsExample
{
	class LegendaryPrompt
	{
		RPCService RPC;
		AutoResetEvent OnConnectEvent;
		bool ConnectionSuccess;

		Dictionary<string, CommandInformation> CommandDictionary;

		Configuration ProgramConfiguration;

		public LegendaryPrompt(Configuration configuration, ConnectionProfile connectionData)
		{
			ProgramConfiguration = configuration;
			RPC = new RPCService(connectionData);
			InitialiseCommandDictionary();
		}

		public void Run()
		{
			OnConnectEvent = new AutoResetEvent(false);
			ConnectionSuccess = false;
			Console.WriteLine("Connecting to server...");
			try
			{
				RPC.Connect(OnConnect);
			}
			catch (Exception exception)
			{
				Console.WriteLine("Connection error: " + exception.Message);
			}
			OnConnectEvent.WaitOne();
			PerformQueries();
		}

		void OnConnect(bool connected)
		{
			ConnectionSuccess = connected;
			if (connected)
				Console.WriteLine("Successfully connected to the server.");
			else
				Console.WriteLine("There was an error connecting to the server.");
			OnConnectEvent.Set();
		}

		void ProcessLine(string line)
		{
			List<string> arguments = line.Split(new char[] {' '}).ToList();
			if (line.Length == 0 || arguments.Count == 0)
				return;

			string command = arguments[0];
			arguments.RemoveAt(0);

			if (!CommandDictionary.ContainsKey(command))
			{
				Console.WriteLine("Unrecognised command, enter \"help\" for a list of commands");
				return;
			}

			CommandInformation commandInformation = CommandDictionary[command];
			if (commandInformation.ArgumentCount != -1 && arguments.Count != commandInformation.ArgumentCount)
			{
				Console.WriteLine("Invalid number of arguments specified");
				return;
			}

			commandInformation.Handler(arguments);
		}

		void PerformQueries()
		{
			while (true)
			{
				Console.Write("> ");
				string line = Console.ReadLine();
				try
				{
					ProcessLine(line);
				}
				catch (Exception exception)
				{
					Console.WriteLine("An exception occurred:");
					Console.WriteLine(exception.Message);
					break;
				}
			}
		}

		void InitialiseCommandDictionary()
		{
			CommandDictionary = new Dictionary<string, CommandInformation>()
			{
				{"help", new CommandInformation(0, PrintHelp, "", "Prints this help")},
				{"id", new CommandInformation(-1, GetAccountID, "<name>", "Retrieve the account ID associated with the given summoner name")},
				{"profile", new CommandInformation(-1, AnalyseSummonerProfile, "<name>", "Retrieve general information about the summoner with the specified name")},
				{"ranked", new CommandInformation(-1, RankedStatistics, "<name>", "Analyse the ranked statistics of the summoner given")},
				{"recent", new CommandInformation(-1, AnalyseRecentGames, "<name>", "Analyse the recent games of the summoner given")},
				{"test", new CommandInformation(-1, RunTest, "<name>", "Perform test")},
			};
		}

		void PrintHelp(List<string> arguments)
		{
			Console.WriteLine("List of commands available:");
			foreach (var entry in CommandDictionary)
			{
				var information = entry.Value;
				Console.WriteLine(entry.Key + " " + information.ArgumentDescription);
				Console.WriteLine("\t" + information.Description);
			}
		}

		string GetNameFromArguments(List<string> arguments)
		{
			string summonerName = "";
			bool first = true;
			foreach (var argument in arguments)
			{
				if (first)
					first = false;
				else
					summonerName += " ";
				summonerName += argument;
			}
			return summonerName;
		}

		void NoSuchSummoner()
		{
			Console.WriteLine("No such summoner");
		}

		void GetAccountID(List<string> arguments)
		{
			string summonerName = GetNameFromArguments(arguments);
			PublicSummoner summoner = RPC.GetSummonerByName(summonerName);
			if (summoner != null)
				Console.WriteLine(summoner.acctId);
			else
				NoSuchSummoner();
		}

		static int CompareGames(PlayerGameStats x, PlayerGameStats y)
		{
			return - x.gameId.CompareTo(y.gameId);
		}

		bool GetNormalElo(List<PlayerGameStats> recentGames, ref int normalElo)
		{
			foreach (var stats in recentGames)
			{
				if (stats.queueType == "NORMAL" && stats.gameMode == "CLASSIC" && !stats.ranked)
				{
					normalElo = stats.rating + stats.eloChange;
					return true;
				}
			}
			return false;
		}

		bool GetRecentGames(string summonerName, ref PublicSummoner publicSummoner, ref List<PlayerGameStats> recentGames)
		{
			publicSummoner = RPC.GetSummonerByName(summonerName);
			if (publicSummoner == null)
				return false;
			RecentGames recentGameData = RPC.GetRecentGames(publicSummoner.acctId);
			recentGames = recentGameData.gameStatistics;
			recentGames.Sort(CompareGames);
			return true;
		}

		void AnalayseStatistics(string description, string target, List<PlayerStatSummary> summaries, bool isNormalElo = false, bool foundNormalElo = false, int normalElo = 0)
		{
			foreach (var summary in summaries)
			{
				if (summary.playerStatSummaryType == target)
				{
					int games = summary.wins + summary.losses;
					if (games == 0)
						continue;
					string intro = description + ": ";
					string winLossAnalysis = summary.wins + " W - " + summary.losses + " L (" + SignPrefix(summary.wins - summary.losses) + ")";
					if (summary.leaves > 0)
						winLossAnalysis += ", left " + summary.leaves + " " + (summary.leaves > 1 ? "games" : "game");
					if (isNormalElo)
					{
						if(foundNormalElo)
							Console.WriteLine(intro + normalElo + ", " + winLossAnalysis);
						else
							Console.WriteLine(intro + "unknown rating, " + winLossAnalysis);
					}
					else
						Console.WriteLine(intro + summary.rating + " (top " + summary.maxRating + "), " + winLossAnalysis);
					return;
				}
			}
		}

		void AnalyseSummonerProfile(List<string> arguments)
		{
			string summonerName = GetNameFromArguments(arguments);
			PublicSummoner publicSummoner = new PublicSummoner();
			List<PlayerGameStats> recentGames = new List<PlayerGameStats>();
			bool foundSummoner = GetRecentGames(summonerName, ref publicSummoner, ref recentGames);
			if (!foundSummoner)
			{
				NoSuchSummoner();
				return;
			}

			PlayerLifeTimeStats lifeTimeStatistics = RPC.RetrievePlayerStatsByAccountID(publicSummoner.acctId, "CURRENT");
			if (lifeTimeStatistics == null)
			{
				Console.WriteLine("Unable to retrieve lifetime statistics");
				return;
			}

			AllSummonerData allSummonerData = RPC.GetAllSummonerDataByAccount(publicSummoner.acctId);
			if (lifeTimeStatistics == null)
			{
				Console.WriteLine("Unable to retrieve all summoner data");
				return;
			}

			int normalElo = 0;
			bool foundNormalElo = GetNormalElo(recentGames, ref normalElo);

			List<PlayerStatSummary> summaries = lifeTimeStatistics.playerStatSummaries.playerStatSummarySet;

			Console.WriteLine("Account ID: " + publicSummoner.summonerId);
			Console.WriteLine("Summoner level: " + publicSummoner.summonerLevel);
			Console.WriteLine("IP: " + allSummonerData.summonerLevelAndPoints.infPoints);

			/*
			 * SK Ocelote:
			 * 
			 * RankedPremade3x3: 0 - 0, 1291 (1791)
			 * RankedTeam3x3: 1 - 0, 1410 (1435)
			 * RankedPremade5x5: 19 - 7, 1751 (2085)
			 * RankedTeam5x5: 69 - 12, 1400 (1442)
			 * 
			 * Actual values from the profile:
			 * 
			 * Arranged team 5v5: 19 - 7, 1751 (2085)
			 * Unranked in 3v3
			 * 
			 * syrela:
			 * 
			 * RankedPremade3x3: 37 - 9, 1715 (1924)
			 * RankedTeam3x3: 53 - 6, 1400 (1400)
			 * RankedPremade5x5: 19 - 6 1611 (1733)
			 * RankedTeam5x5: 26 - 4, 1400 (1400)
			 * 
			 * Actual values from the profile:
			 * 
			 * Arranged team 3v3: 37 - 9, 1715 (1924)
			 * Arranged team 5v5: 19 - 6, 1611 (1733)
			 * 
			 * Doesn't match any data from last season either.
			 * This means that all the "Team" variants are unused right now.
			 */


			AnalayseStatistics("Unranked Summoner's Rift", "Unranked", summaries, true, foundNormalElo, normalElo);
			AnalayseStatistics("Ranked Twisted Treeline (team)", "RankedPremade3x3", summaries);
			AnalayseStatistics("Ranked Summoner's Rift (solo)", "RankedSolo5x5", summaries);
			AnalayseStatistics("Ranked Summoner's Rift (team)", "RankedPremade5x5", summaries);
			AnalayseStatistics("Unranked Dominion", "OdinUnranked", summaries);
		}

		string GetPrefix(int input)
		{
			if (input >= 0)
				return "+";
			return "-";
		}

		string SignPrefix(int input)
		{
			return GetPrefix(input) + Math.Abs(input).ToString();
		}

		void AnalyseRecentGames(List<string> arguments)
		{
			string summonerName = GetNameFromArguments(arguments);
			PublicSummoner publicSummoner = new PublicSummoner();
			List<PlayerGameStats> recentGames = new List<PlayerGameStats>();
			bool foundSummoner = GetRecentGames(summonerName, ref publicSummoner, ref recentGames);
			if (!foundSummoner)
			{
				NoSuchSummoner();
				return;
			}
			foreach (var stats in recentGames)
			{
				Console.Write("[" + stats.gameId + "] ");
				if (stats.ranked)
					Console.Write("Ranked ");
				if (stats.gameType == "PRACTICE_GAME")
				{
					Console.Write("Custom ");
					switch (stats.gameMode)
					{
						case "CLASSIC":
							Console.Write("Summoner's Rift");
							break;

						case "ODIN":
							Console.Write("Dominion");
							break;

						default:
							Console.Write(stats.gameMode);
							break;
					}
				}
				else
				{
					switch (stats.queueType)
					{
						case "RANKED_TEAM_3x3":
							Console.WriteLine("Twisted Treeline");
							break;

						case "NORMAL":
						case "RANKED_SOLO_5x5":
							Console.Write("Summoner's Rift");
							break;

						case "RANKED_TEAM_5x5":
							Console.Write("Summoner's Rift (team)");
							break;

						case "ODIN_UNRANKED":
							Console.Write("Dominion");
							break;

						case "BOT":
							Console.Write("Co-op vs. AI");
							break;

						default:
							Console.Write(stats.queueType);
							break;
					}
				}
				if (stats.adjustedRating != 0)
				{
					//Sometimes the servers are bugged and show invalid rating values
					//Console.Write(", " + stats.rating + " " + GetPrefix(stats.eloChange) + " " + Math.Abs(stats.eloChange) + " = " + (stats.rating + stats.eloChange));
					Console.Write(", " + (stats.rating + stats.eloChange) + " (" + SignPrefix(stats.eloChange) + ")");
				}
				if (stats.adjustedRating != 0)
					Console.Write(", adjusted " + stats.adjustedRating);
				if (stats.teamRating != 0)
					Console.Write(", team " + stats.teamRating + " (" + SignPrefix(stats.teamRating - stats.rating) + ")");
				if (stats.predictedWinPct != 0)
					Console.Write(", prediction " + Percentage(stats.predictedWinPct));
				if (stats.premadeSize > 1)
					Console.Write(", queued with " + stats.premadeSize);
				Console.WriteLine("");
			}
		}

		string Percentage(double input)
		{
			return string.Format("{0:0.0%}", input);
		}

		string Round(double input)
		{
			return string.Format("{0:0.0}", input);
		}

		List<ChampionStatistics> TranslateAggregatedStatistics(AggregatedStats statistics)
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

		int CompareNames(ChampionStatistics x, ChampionStatistics y)
		{
			return x.Name.CompareTo(y.Name);
		}

		void RankedStatistics(List<string> arguments)
		{
			string summonerName = GetNameFromArguments(arguments);
			PublicSummoner publicSummoner = RPC.GetSummonerByName(summonerName);
			if (publicSummoner == null)
			{
				NoSuchSummoner();
				return;
			}
			AggregatedStats aggregatedStatistics = RPC.GetAggregatedStats(publicSummoner.acctId, "CLASSIC", "CURRENT");
			List<ChampionStatistics> statistics = TranslateAggregatedStatistics(aggregatedStatistics);
			foreach (var entry in statistics)
			{
				if (ProgramConfiguration.ChampionNames.ContainsKey(entry.ChampionId))
					entry.Name = ProgramConfiguration.ChampionNames[entry.ChampionId];
				else
					entry.Name = "Champion " + entry.ChampionId;
			}
			statistics.Sort(CompareNames);
			foreach (var entry in statistics)
				Console.WriteLine(entry.Name + ": " + entry.Victories + " W - " + entry.Defeats + " L (" + SignPrefix(entry.Victories - entry.Defeats) + "), " + Percentage(entry.WinRatio()) + ", " + Round(entry.KillsPerGame()) + "/" + Round(entry.DeathsPerGame()) + "/" + Round(entry.AssistsPerGame()) + ", " + Round(entry.KillsAndAssistsPerDeath()));
		}

		void RunTest(List<string> arguments)
		{
			string summonerName = GetNameFromArguments(arguments);
			PublicSummoner publicSummoner = RPC.GetSummonerByName(summonerName);
			if (publicSummoner == null)
			{
				NoSuchSummoner();
				return;
			}
			AggregatedStats result = RPC.GetAggregatedStats(publicSummoner.acctId, "CLASSIC", "CURRENT");
			Console.WriteLine("Successs");
		}
	}
}
