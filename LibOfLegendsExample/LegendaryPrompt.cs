using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FluorineFx.Net;

using LibOfLegends;

using com.riotgames.platform.summoner;

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
				{"id", new CommandInformation(-1, GetAccountID, "<name>", "Retrieve the account ID of the given summoner name")},
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

		void GetAccountID(List<string> arguments)
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
			PublicSummoner summoner = RPC.GetSummonerByName(summonerName);
			if (summoner != null)
				Console.WriteLine(summoner.acctId);
			else
				Console.WriteLine("No such summoner");
		}
	}
}
