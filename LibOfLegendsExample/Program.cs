using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using FluorineFx;
using FluorineFx.Net;

using LibOfLegends;



namespace LolineFX
{
    class Program
    {
        private static RPCService _rpc = new RPCService(RegionTag.EUW);

        static void Main(string[] arguments)
        {
			if (arguments.Length != 2)
			{
				System.Console.WriteLine("Usage:");
				System.Console.WriteLine(Environment.GetCommandLineArgs()[0] + " <user> <password>");
				return;
			}
            _rpc.Connect(arguments[0], arguments[1], OnConnect);

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
                string command = Console.ReadLine();

                try
                {

                    Command c = new Command(command);

                    switch (c.Name)
                    {
                        case "GetRecentGamesByName":
                        {
                            GetRecentGamesByNameContext games = new GetRecentGamesByNameContext(_rpc, c.Arguments[0]);
                            games.Execute();

                            Console.WriteLine(games);
                            break;
                        }
                        case "GetRecentGames":
                        {
                            GetRecentGamesContext games = new GetRecentGamesContext(_rpc, int.Parse(c.Arguments[0]));
                            games.Execute();

                            Console.WriteLine(games);
                            break;
                        }
                        case "GetSummonerByName":
                        {
                            GetSummonerByNameContext name = new GetSummonerByNameContext(_rpc, c.Arguments[0]);
                            name.Execute();

                            Console.WriteLine(name);
                            break;
                        }
                        case "GetAllPublicSummonerDataByAccount":
                        {
                            GetAllPublicSummonerDataByAccountContext publicData = new GetAllPublicSummonerDataByAccountContext(_rpc, int.Parse(c.Arguments[0]));
                            publicData.Execute();

                            Console.WriteLine(publicData);
                            break;
                        }
                        case "GetAllSummonerDataByAccount":
                        {
                            GetAllSummonerDataByAccountContext data = new GetAllSummonerDataByAccountContext(_rpc, int.Parse(c.Arguments[0]));
                            data.Execute();

                            Console.WriteLine(data);
                            break;
                        }
                        case "RetrievePlayerStatsByAccountID":
                        {
                            RetrievePlayerStatsByAccountIdContext playerStats = new RetrievePlayerStatsByAccountIdContext(_rpc, int.Parse(c.Arguments[0]), c.Arguments[1]);
                            playerStats.Execute();

                            Console.WriteLine(playerStats);
                            break;
                        }
                        case "GetAggregatedStats":
                        {
                            GetAggregatedStatsContext aggregatedStats = new GetAggregatedStatsContext(_rpc, int.Parse(c.Arguments[0]), c.Arguments[1], c.Arguments[2]);
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
                            Console.WriteLine("Unrecognised command, type ? or help for a list of commands");
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error executing command");
                    Console.WriteLine("------------------------------------------------");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine("------------------------------------------------");
                }
                Console.WriteLine();
            }
        }
    }

    
}
