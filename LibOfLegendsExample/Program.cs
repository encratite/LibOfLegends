using System;
using System.Collections.Generic;
using System.Threading;

using FluorineFx;
using FluorineFx.Net;

using LibOfLegends;



namespace LolineFX
{
    class GetRecentGamesByNameContext
    {
        public GetRecentGamesByNameContext(RPCService service, string name)
        {
            _service = service;
            _name = name;
        }

        public void Retrieve()
        {
            _service.GetSummonerByName(_name, new Responder<object>(OnGetSummonerByName));
        }

        public void OnGetSummonerByName(object o)
        {
            Console.WriteLine("\t<GetSummonerByName>");
            try
            {
                Dictionary<string, object> dictionary = (Dictionary<string, object>)o;
                _accountID = Convert.ToInt32(dictionary["acctId"]);
                _service.GetRecentGames(_accountID, new Responder<object>(OnGetRecentGames));
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}\n{1}", e.Message, e.StackTrace);
            }
            Console.WriteLine("\t</GetSummonerByName>");
        }

        public void OnGetRecentGames(object o)
        {
            Console.WriteLine("\tGetRecentGames");
            Dictionary<string, object> dictionary = (Dictionary<string, object>)o;

            RecentGames = new List<object>();

            foreach (object gameStat in (FluorineFx.AMF3.ArrayCollection)dictionary["gameStatistics"])
                RecentGames.Add(gameStat);
            Console.WriteLine("\t</GetRecentGames>");
        }

        public List<object> RecentGames;

        private RPCService _service;
        private string _name;
        private int _accountID;
    }

    class Program
    {
        private static RPCService _rpc = new RPCService(RegionTag.EUW);

        static void Main(string[] args)
        {
            _rpc.Connect(args[0], args[1], OnConnect );

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

        static void PerformQueries()
        {
            while (true)
            {
                Console.Write("Please input a summoner name: ");
                string summoner = Console.ReadLine();

                Console.WriteLine("Getting recent games for {0}", summoner);
                GetRecentGamesByNameContext c = new GetRecentGamesByNameContext(_rpc, summoner);
                c.Retrieve();

                // Wait some time. This should be replaced with something more sensible!
                // Like the monitor code I deleted because I was convinced it was broken.
                System.Threading.Thread.Sleep(10000);

                Console.WriteLine("Printing stuff now.");
                foreach (object o in c.RecentGames)
                {
                    Dictionary<string, object> dictionary = o as Dictionary<string, object>;
                    Console.WriteLine("\tNew match");
                    foreach(KeyValuePair<string, object> kvp in dictionary)
                    {
                        if (kvp.Value is string || kvp.Value is int || kvp.Value is bool || kvp.Value is double)
                            Console.WriteLine("\t\t{0}, {1}", kvp.Key, kvp.Value);
                    }
                }

                Console.WriteLine();
            }
        }
    }

    
}
