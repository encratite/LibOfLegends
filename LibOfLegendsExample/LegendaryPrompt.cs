using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FluorineFx.Net;

using LibOfLegends;

using com.riotgames.platform.summoner;
using com.riotgames.platform.statistics;

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
				{"id", new CommandInformation(-1, GetAccountID, "<name>", "Retrieve the account ID associated with the given summoner name")},
				{"analyse", new CommandInformation(-1, AnalyseRecentGames, "<name>", "Analyse the recent games of the summoner given")},
				{"help", new CommandInformation(0, PrintHelp, "", "Prints this help")},
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

		void AnalyseRecentGames(List<string> arguments)
		{
			string summonerName = GetNameFromArguments(arguments);
			PublicSummoner summoner = RPC.GetSummonerByName(summonerName);
			if (summoner == null)
			{
				NoSuchSummoner();
				return;
			}
			RecentGames recentGameData = RPC.GetRecentGames(summoner.acctId);
			var recentGames = recentGameData.gameStatistics;
			recentGames.Sort(CompareGames);
			int normalElo = 0;
			bool foundNormalElo = false;
			foreach (var stats in recentGames)
			{
				if (stats.queueType == "NORMAL" && stats.gameMode == "CLASSIC" && !stats.ranked)
				{
					normalElo = stats.rating + stats.eloChange;
					foundNormalElo = true;
					break;
				}
			}
			if (foundNormalElo)
				Console.WriteLine("Normal Elo: " + normalElo);
			else
				Console.WriteLine("No normal games in match history");
		}
	}
}
