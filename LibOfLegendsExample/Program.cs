using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using FluorineFx;
using FluorineFx.Net;

using LibOfLegends;

namespace LibOfLegendsExample
{
    class Program
    {
		private const string ConfigurationFile = "Configuration.xml";
        private static RPCService RPC;

        static void Main(string[] arguments)
        {
			Configuration configuration;
			try
			{
				Serialiser<Configuration> serialiser = new Serialiser<Configuration>(ConfigurationFile);
				configuration = serialiser.Load();
			}
			catch (System.IO.FileNotFoundException)
			{
				System.Console.WriteLine("Unable to load configuration file \"" + ConfigurationFile + "\"");
				return;
			}
			catch (System.InvalidOperationException)
			{
				System.Console.WriteLine("Malformed configuration file");
				return;
			}

			if (arguments.Length != 3)
			{
				System.Console.WriteLine("Usage:");
				System.Console.WriteLine(Environment.GetCommandLineArgs()[0] + " <server> <user> <password>");
				System.Console.Write("Servers available:");
				foreach (ServerProfile profile in configuration.ServerProfiles)
					System.Console.Write(" " + profile.Abbreviation);
				System.Console.WriteLine("");
				return;
			}

			string server = arguments[0];
			string user = arguments[1];
			string password = arguments[2];

			ServerProfile chosenProfile = null;
			foreach (ServerProfile profile in configuration.ServerProfiles)
			{
				if (profile.Abbreviation.ToLower() == server.ToLower())
				{
					chosenProfile = profile;
					break;
				}
			}

			if (chosenProfile == null)
			{
				System.Console.WriteLine("Unable to find server profile \"" + server + "\"");
				return;
			}

			RegionData regionData = new RegionData(chosenProfile.LoginQueueURL, chosenProfile.RPCURL);
			ConnectionData connectionData = new ConnectionData(regionData, user, password);

			RPC = new RPCService(connectionData);
            RPC.Connect(OnConnect);

            // Eugh.
            while(true)
                System.Threading.Thread.Sleep(100);
        }

        static void OnConnect(bool connected)
        {
            if (!connected)
            {
                Console.WriteLine("There was an error connecting to the server.");
                return;
            }
            
            Console.WriteLine("Successfully connected to the server, you are now free to query stuff!");
            Thread t = new Thread(PerformQueries);
            t.Start();
        }

        static void GetRecentGamesByName(string name)
        {
        }

        static void PerformQueries()
        {
            while (true)
            {
                Console.Write("[Legendary Prompt] ");
                string commandString = Console.ReadLine();

                try
                {
                    Command command = new Command(commandString);

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
        }
    }

    
}
