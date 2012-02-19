using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FluorineFx.Net;

using LibOfLegends;

namespace LibOfLegendsExample
{
	class LegendaryPrompt
	{
		RPCService RPC;
		AutoResetEvent OnConnectEvent;
		AutoResetEvent CommandEvent;
		bool ConnectionSuccess;

		public LegendaryPrompt(ConnectionProfile connectionData)
		{
			RPC = new RPCService(connectionData);
			CommandEvent = new AutoResetEvent(false);
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
			Dictionary<string, CommandInformation> CommandDictionary = new Dictionary<string, CommandInformation>()
			{
				{"GetSummonerByName", new CommandInformation(1, GetSummonerByName, "<name>", "name\n\tRetrieve the summoner data for a given name")},
			};

			List<string> arguments = line.Split(new string[] { " " }, 2, StringSplitOptions.None).ToList();
			if (line.Length == 0 || arguments.Count == 0)
				return;

			string command = arguments[0];
			arguments.RemoveAt(0);

			if (!CommandDictionary.ContainsKey(command))
			{
				Console.WriteLine("Unrecognised command, type \"?\" or \"help\" for a list of commands");
				return;
			}

			CommandInformation commandInformation = CommandDictionary[command];
			if (arguments.Count != commandInformation.ArgumentCount)
			{
				Console.WriteLine("Invalid number of arguments specified");
				return;
			}

			commandInformation.Handler(arguments);
		}

		void ProcessLineOld(string line)
		{
			try
			{
				Command command = new Command(line);

				switch (command.Name)
				{
					case "GetRecentGamesByName":
					{
						GetRecentGamesByNameContext games = new GetRecentGamesByNameContext(RPC, command.Arguments[0]);
						games.Execute();

						Console.WriteLine(games);
						break;
					}
					case "GetRecentGames":
					{
						GetRecentGamesContext games = new GetRecentGamesContext(RPC, int.Parse(command.Arguments[0]));
						games.Execute();

						Console.WriteLine(games);
						break;
					}
					case "GetSummonerByName":
					{
						GetSummonerByNameContext name = new GetSummonerByNameContext(RPC, command.Arguments[0]);
						name.Execute();

						Console.WriteLine(name);
						break;
					}
					case "GetAllPublicSummonerDataByAccount":
					{
						GetAllPublicSummonerDataByAccountContext publicData = new GetAllPublicSummonerDataByAccountContext(RPC, int.Parse(command.Arguments[0]));
						publicData.Execute();

						Console.WriteLine(publicData);
						break;
					}
					case "GetAllSummonerDataByAccount":
					{
						GetAllSummonerDataByAccountContext data = new GetAllSummonerDataByAccountContext(RPC, int.Parse(command.Arguments[0]));
						data.Execute();

						Console.WriteLine(data);
						break;
					}
					case "RetrievePlayerStatsByAccountID":
					{
						RetrievePlayerStatsByAccountIdContext playerStats = new RetrievePlayerStatsByAccountIdContext(RPC, int.Parse(command.Arguments[0]), command.Arguments[1]);
						playerStats.Execute();

						Console.WriteLine(playerStats);
						break;
					}
					case "GetAggregatedStats":
					{
						GetAggregatedStatsContext aggregatedStats = new GetAggregatedStatsContext(RPC, int.Parse(command.Arguments[0]), command.Arguments[1], command.Arguments[2]);
						aggregatedStats.Execute();

						Console.WriteLine(aggregatedStats);
						break;
					}
					case "?":
					case "help":
					{
						Command.PrintHelp();
						break;
					}
					default:
					{
						Console.WriteLine("Unrecognised command, type \"?\" or \"help\" for a list of commands");
						break;
					}
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine("Error executing command");
				Console.WriteLine("------------------------------------------------");
				Console.WriteLine(exception.Message);
				Console.WriteLine(exception.StackTrace);
				Console.WriteLine("------------------------------------------------");
			}
			Console.WriteLine();
		}

		void PerformQueries()
        {
			while (true)
			{
				Console.Write("[Legendary Prompt] ");
				string line = Console.ReadLine();
				//ProcessLine(line);
				ProcessLineOld(line);
			}
        }

		void Wait()
        {
			CommandEvent.WaitOne();
        }

        void Signal()
        {
			CommandEvent.Set();
        }

		void GetSummonerByName(List<string> arguments)
		{
			string summonerName = arguments[0];
			RPC.GetSummonerByName(summonerName, new Responder<object>(OnGetSummonerByName));
            Wait();
		}

		void OnGetSummonerByName(object result)
		{
			Console.WriteLine("Success");
			Signal();
		}
	}
}
