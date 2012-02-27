using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FluorineFx.Net;

using LibOfLegends;

using com.riotgames.platform.summoner;
using com.riotgames.platform.statistics;
using com.riotgames.platform.gameclient.domain;

namespace LibOfLegendsExample
{
	class LegendaryPrompt
	{
		RPCService RPC;
		AutoResetEvent OnConnectEvent;
		bool ConnectionSuccess;

		Dictionary<string, CommandInformation> CommandDictionary;

		public LegendaryPrompt(ConnectionProfile connectionData)
		{
			RPC = new RPCService(connectionData);
			InitialiseCommandDictionary();
		}

		public void Run()
		{
			OnConnectEvent = new AutoResetEvent(false);
			ConnectionSuccess = false;
			Console.WriteLine("Connecting to server...");
			RPC.Connect(OnConnect);
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
				ProcessLine(line);
			}
		}

		void InitialiseCommandDictionary()
		{
			CommandDictionary = new Dictionary<string, CommandInformation>()
			{
				{"help", new CommandInformation(0, PrintHelp, "", "Prints this help")},
				{"id", new CommandInformation(-1, GetAccountID, "<name>", "Retrieve the account ID associated with the given summoner name")},
				{"profile", new CommandInformation(-1, AnalyseSummonerProfile, "<name>", "Retrieve general information about the summoner with the specified name")},
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

			int normalElo = 0;
			bool foundNormalElo = GetNormalElo(recentGames, ref normalElo);

			Console.WriteLine("Account ID: " + publicSummoner.summonerId);
			Console.WriteLine("Summoner level: " + publicSummoner.summonerLevel);

			if (foundNormalElo)
				Console.WriteLine("Normal Elo: " + normalElo);
			else
				Console.WriteLine("No normal games in match history");
		}

		string GetPrefix(int input)
		{
			if (input > 0)
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
					Console.Write(", prediction " + string.Format("{0:0.0%}", stats.predictedWinPct));
				if (stats.premadeSize > 1)
					Console.Write(", queued with " + stats.premadeSize);
				Console.WriteLine("");
			}
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
			AllPublicSummonerDataDTO publicSummonerData = RPC.GetAllPublicSummonerDataByAccount(publicSummoner.acctId);
			//Console.WriteLine(publicSummonerData.summonerDefaultSpells.summonerDefaultSpellMap.Count);
			Console.WriteLine("Successs");
		}
	}
}
