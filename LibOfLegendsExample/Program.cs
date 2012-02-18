using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using FluorineFx;
using FluorineFx.Net;

using LibOfLegends;



namespace LolineFX
{
    #region Command Object
    public class ArgumentListException : ApplicationException
    {
    }

    public class InvalidCommandException : ApplicationException
    {
    }

    public class Command
    {
        public Command(string command)
        {
            string[] parts = command.Split(new string[] {" "}, 2, StringSplitOptions.None);

            if (parts.Length == 0)
                throw new InvalidCommandException();

            Name = parts[0];

            if (!ExpectedArgs.ContainsKey(Name))
                throw new InvalidCommandException();

            Arguments = Utility.CommandLineToArgs(parts[1]).ToList();
            if (Arguments.Count != ExpectedArgs[Name])
                throw new InvalidCommandException();
        }

        public Dictionary<string, int> ExpectedArgs = new Dictionary<string, int>()
        {
            {"GetSummonerByName", 1},
            {"GetRecentGames", 1},
            {"GetAllPublicSummonerDataByAccount", 1},
            {"GetAllSummonerDataByAccount", 1},
            {"GetRecentGamesByName", 1}
        };

        public string Name;
        public List<string> Arguments;
    }
    #endregion

    #region RPC Contexts

    public abstract class ExecutionContext
    {
        public ExecutionContext(RPCService service)
        {
            _service = service;
        }

        public abstract void Execute();

        protected void _Wait()
        {
            lock(this)
                Monitor.Wait(this);
        }

        protected void _Signal()
        {
            lock (this)
                Monitor.Pulse(this);
        }

        protected RPCService _service;
    }

    public class GetSummonerByNameContext : ExecutionContext
    {
        public GetSummonerByNameContext(RPCService service, string name) : base(service)
        {
            _name = name;
            AccountID = -1;
        }

        public override void Execute()
        {
            _service.GetSummonerByName(_name, new Responder<object>(_OnGetSummonerByName));
            _Wait();
        }

        private void _OnGetSummonerByName(object o)
        {
            Dictionary<string, object> dictionary = (Dictionary<string, object>)o;
            AccountID = Convert.ToInt32(dictionary["acctId"]);
            _Signal();
        }

        public override string ToString()
        {
            return AccountID.ToString();
        }

        private string _name;
        public int AccountID { get; set; }
    }

    public class GetRecentGamesContext : ExecutionContext
    {
        public GetRecentGamesContext(RPCService service, int accountID)
            : base(service)
        {
            _accountID = accountID;
        }

        public override void Execute()
        {
            _service.GetRecentGames(_accountID, new Responder<object>(_OnGetRecentGames));
            _Wait();
        }

        private void _OnGetRecentGames(object o)
        {
            Dictionary<string, object> dictionary = (Dictionary<string, object>) o;

            foreach (object gameStat in (FluorineFx.AMF3.ArrayCollection)dictionary["gameStatistics"])
                RecentGames.Add((Dictionary<string, object>)gameStat);

            // Convert this to a more strongly typed list and sort it by gameId.
            RecentGames = RecentGames.ConvertAll(i => (Dictionary<string, object>)i);
            RecentGames = RecentGames.OrderBy(g => g["gameId"]).ToList();

            // Signal that the games have been retrieved.
            _Signal();
        }

        public override string ToString()
        {
            string text = string.Empty;

            foreach (Dictionary<string, object> d in RecentGames)
            {
	            text += "\tNew match\n";
	            foreach(KeyValuePair<string, object> kvp in d)
	            {
	                if (kvp.Value is string || kvp.Value is int || kvp.Value is bool || kvp.Value is double)
	                    text += string.Format("\t\t{0}, {1}\n", kvp.Key, kvp.Value);
	            }
            }

            return text;
        }

        public List<Dictionary<string, object>> RecentGames = new List<Dictionary<string, object>>();

        private int _accountID;
    }

    public class GetAllPublicSummonerDataByAccountContext : ExecutionContext
    {
        public GetAllPublicSummonerDataByAccountContext(RPCService service, int accountID)
            : base(service)
        {
            SummonerData = null;
            _accountID = accountID;
        }

        public override void Execute()
        {
            _service.GetAllPublicSummonerDataByAccount(_accountID, new Responder<object>(_OnGetAllPublicSummonerData));
            _Wait();
        }

        private void _OnGetAllPublicSummonerData(object o)
        {
            SummonerData = o as Dictionary<string, object>;
            _Signal();
        }

        public override string ToString()
        {
            return SummonerData.PrettyPrint();
        }

        Dictionary<string, object> SummonerData;
        private int _accountID;
    }

    public class GetAllSummonerDataByAccountContext : ExecutionContext
    {
        public GetAllSummonerDataByAccountContext(RPCService service, int accountID)
            : base(service)
        {
            SummonerData = null;
            _accountID = accountID;
        }

        public override void Execute()
        {
            _service.GetAllSummonerDataByAccount(_accountID, new Responder<object>(_OnGetAllSummonerData));
            _Wait();
        }

        private void _OnGetAllSummonerData(object o)
        {
            SummonerData = o as Dictionary<string, object>;
            _Signal();
        }

        public override string ToString()
        {
            return SummonerData.PrettyPrint();
        }

        Dictionary<string, object> SummonerData;
        private int _accountID;
    }

    public class GetRecentGamesByNameContext : ExecutionContext
    {

        public GetRecentGamesByNameContext(RPCService service, string name) : base(service)
        {
            _name = name;
            _getSummonerByNameContext = null;
            _getRecentGamesContext = null;
        }

        public override void Execute()
        {
            Console.WriteLine("Getting recent games for {0}", _name);

            _getSummonerByNameContext = new GetSummonerByNameContext(_service, _name);
            _getSummonerByNameContext.Execute();

            _getRecentGamesContext = new GetRecentGamesContext(_service, _getSummonerByNameContext.AccountID);
            _getRecentGamesContext.Execute();
            
            if (_getRecentGamesContext.RecentGames == null)
                throw new ApplicationException(string.Format("Error retrieving games for {0}", _name));
        }

        public List<Dictionary<string, object>> RecentGames
        {
            get { return _getRecentGamesContext.RecentGames; }
        }

        public override string ToString()
        {
            return _getRecentGamesContext.ToString();
        }

        private string _name;

        private GetSummonerByNameContext _getSummonerByNameContext; 
        private GetRecentGamesContext _getRecentGamesContext;
    }

    #endregion

    class Program
    {
        private static RPCService _rpc = new RPCService(RegionTag.EUW);

        static void Main(string[] args)
        {
            _rpc.Connect(args[0], args[1], OnConnect);

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
