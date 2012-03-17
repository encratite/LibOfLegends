using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using FluorineFx.Net;

using Nil;

using LibOfLegends;

using com.riotgames.platform.statistics;
using com.riotgames.platform.summoner;

namespace LibOfLegendsExample
{
	class LegendaryPrompt
	{
		bool Running;

		RPCService RPC;
		AutoResetEvent OnConnectEvent;
		bool ConnectionSuccess;

		Dictionary<string, CommandInformation> CommandDictionary;

		Configuration ProgramConfiguration;

		ConnectionProfile ConnectionData;

		public LegendaryPrompt(Configuration configuration, ConnectionProfile connectionData)
		{
			ProgramConfiguration = configuration;
			ConnectionData = connectionData;
			InitialiseCommandDictionary();
			Running = true;
		}

		public void Run()
		{
			while (Running)
			{
				OnConnectEvent = new AutoResetEvent(false);
				ConnectionSuccess = false;
				WriteWithTimestamp("Connecting to server...");
				try
				{
					RPC = new RPCService(ConnectionData, OnConnect, OnDisconnect, OnNetStatus);
					RPC.Connect();
				}
				catch (Exception exception)
				{
					Output.WriteLine("Connection error: " + exception.Message);
				}
				OnConnectEvent.WaitOne();
				if (ConnectionSuccess)
					PerformQueries();
			}
		}

		void WriteWithTimestamp(string input, params object[] arguments)
		{
			string message = string.Format("[{0}] {1}", Time.Timestamp(), input);
			Output.WriteLine(message, arguments);
		}

		void OnConnect(RPCConnectResult result)
		{
			ConnectionSuccess = result.Success();
			Output.WriteLine(result.GetMessage());
			OnConnectEvent.Set();
		}

		void OnDisconnect()
		{
			WriteWithTimestamp("Disconnected");
		}

		void OnNetStatus(NetStatusEventArgs eventArguments)
		{
			/*
			WriteWithTimestamp("OnNetStatus:");
			foreach (var pair in eventArguments.Info)
					Output.WriteLine("{0}: {1}", pair.Key, pair.Value);
			*/
		}

		void ProcessLine(string line)
		{
			List<string> arguments = line.Split(new char[] {' '}).ToList();
			if (line.Length == 0 || arguments.Count == 0)
				return;

			string command = arguments[0];
			arguments.RemoveAt(0);

			CommandInformation commandInformation;
			if (!CommandDictionary.TryGetValue(command, out commandInformation))
			{
				Output.WriteLine("Unrecognised command, enter \"help\" for a list of commands");
				return;
			}


			if (commandInformation.ArgumentCount != -1 && arguments.Count != commandInformation.ArgumentCount)
			{
				Output.WriteLine("Invalid number of arguments specified");
				return;
			}

			commandInformation.Handler(arguments);
		}

		void PerformQueries()
		{
			while (Running)
			{
				Console.Write("> ");
				string line = Console.ReadLine();
				try
				{
					ProcessLine(line);
				}
				catch (RPCTimeoutException)
				{
					Output.WriteLine("RPC timeout occurred");
					RPC.Disconnect();
					return;
				}
				catch (Exception exception)
				{
					Output.WriteLine("An exception occurred:");
					Output.WriteLine(exception.Message);
					break;
				}
			}
		}

		void InitialiseCommandDictionary()
		{
			CommandDictionary = new Dictionary<string, CommandInformation>()
			{
				{"quit", new CommandInformation(0, Quit, "", "Terminates the application")},
				{"help", new CommandInformation(0, PrintHelp, "", "Prints this help")},
				{"id", new CommandInformation(-1, GetAccountID, "<name>", "Retrieve the account ID associated with the given summoner name")},
				{"profile", new CommandInformation(-1, AnalyseSummonerProfile, "<name>", "Retrieve general information about the summoner with the specified name")},
				{"ranked", new CommandInformation(-1, RankedStatistics, "<name>", "Analyse the ranked statistics of the summoner given")},
				{"recent", new CommandInformation(-1, AnalyseRecentGames, "<name>", "Analyse the recent games of the summoner given")},
			};
		}

		void Quit(List<string> arguments)
		{
			RPC.Disconnect();
			Running = false;
		}

		void PrintHelp(List<string> arguments)
		{
			Output.WriteLine("List of commands available:");
			foreach (var entry in CommandDictionary)
			{
				var information = entry.Value;
				Output.WriteLine(entry.Key + " " + information.ArgumentDescription);
				Output.WriteLine("\t" + information.Description);
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
			Output.WriteLine("No such summoner");
		}

		void GetAccountID(List<string> arguments)
		{
			string summonerName = GetNameFromArguments(arguments);
			PublicSummoner summoner = RPC.GetSummonerByName(summonerName);
			if (summoner != null)
				Output.WriteLine(summoner.acctId.ToString());
			else
				NoSuchSummoner();
		}

		static int CompareGames(PlayerGameStats x, PlayerGameStats y)
		{
			return - x.createDate.CompareTo(y.createDate);
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
							Output.WriteLine(intro + normalElo + ", " + winLossAnalysis);
						else
							Output.WriteLine(intro + "unknown rating, " + winLossAnalysis);
					}
					else
						Output.WriteLine(intro + summary.rating + " (top " + summary.maxRating + "), " + winLossAnalysis);
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
				Output.WriteLine("Unable to retrieve lifetime statistics");
				return;
			}

			int normalElo = 0;
			bool foundNormalElo = GetNormalElo(recentGames, ref normalElo);

			List<PlayerStatSummary> summaries = lifeTimeStatistics.playerStatSummaries.playerStatSummarySet;

			Output.WriteLine("Name: " + publicSummoner.name);
			Output.WriteLine("Account ID: " + publicSummoner.summonerId);
			Output.WriteLine("Summoner level: " + publicSummoner.summonerLevel);
			//No idea what this value contains now
			//Output.WriteLine("IP: " + allSummonerData.summonerLevelAndPoints.infPoints);

			//The hidden "Team" variants of the "Premade" ratings are currently unused, it seems
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
				GameResult result = new GameResult(stats);
				Console.Write("[{0}] [{1}] [{2}] ", stats.gameId, stats.createDate, result.Win ? "W" : "L");
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
							Output.Write("Twisted Treeline");
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
                Console.WriteLine(", {0}, {1}/{2}/{3}", GetChampionName(stats.championId), result.Kills, result.Deaths, result.Assists);
                List<string> units = new List<string>();
				if (stats.adjustedRating != 0)
					units.Add(string.Format("Rating: {0} ({1})", stats.rating + stats.eloChange, SignPrefix(stats.eloChange)));
				if (stats.adjustedRating != 0)
					units.Add(string.Format("adjusted {0}", stats.adjustedRating));
				if (stats.teamRating != 0)
					units.Add(string.Format("team {0} ({1})", stats.teamRating, SignPrefix(stats.teamRating - stats.rating)));
                PrintUnits(units);
				if (stats.predictedWinPct != 0.0)
					units.Add(string.Format("Predicted winning percentage {0}", Percentage(stats.predictedWinPct)));
				if (stats.premadeSize > 1)
					units.Add(string.Format("Queued with {0}", stats.premadeSize));
				if (stats.leaver)
					units.Add("Left the game");
				if (stats.afk)
					units.Add("AFK");
				units.Add(string.Format("{0} ms ping", stats.userServerPing));
				units.Add(string.Format("{0} s spent in queue", stats.timeInQueue));
                PrintUnits(units);
			}
		}

        void PrintUnits(List<string> units)
        {
            if (units.Count > 0)
                Output.WriteLine(string.Join(", ", units));
            units.Clear();
        }

		string Percentage(double input)
		{
			return string.Format("{0:0.0%}", input);
		}

		string Round(double input)
		{
			return string.Format("{0:0.0}", input);
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
			if (aggregatedStatistics == null)
			{
				Output.WriteLine("Unable to retrieve aggregated statistics");
				return;
			}
			List<ChampionStatistics> statistics = ChampionStatistics.GetChampionStatistics(aggregatedStatistics);
            foreach (var entry in statistics)
                entry.Name = GetChampionName(entry.ChampionId);
			statistics.Sort(CompareNames);
			foreach (var entry in statistics)
				Output.WriteLine(entry.Name + ": " + entry.Wins + " W - " + entry.Losses + " L (" + SignPrefix(entry.Wins - entry.Losses) + "), " + Percentage(entry.WinRatio()) + ", " + Round(entry.KillsPerGame()) + "/" + Round(entry.DeathsPerGame()) + "/" + Round(entry.AssistsPerGame()) + ", " + Round(entry.KillsAndAssistsPerDeath()));
		}

        string GetChampionName(int championId)
        {
            string name;
            if (!ProgramConfiguration.ChampionNames.TryGetValue(championId, out name))
                name = string.Format("Champion {0}", championId);
            return name;
        }
	}
}
