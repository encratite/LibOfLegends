using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FluorineFx.Net;

using LibOfLegends;

namespace LibOfLegendsExample
{
    public abstract class ExecutionContext
    {
        public ExecutionContext(RPCService service)
        {
            _service = service;
        }

        public abstract void Execute();

        protected void _Wait()
        {
            _event.WaitOne();
        }

        protected void _Signal()
        {
            _event.Set();
        }

        AutoResetEvent _event = new AutoResetEvent(false);
        protected RPCService _service;
    }

    public class GetSummonerByNameContext : ExecutionContext
    {
        public GetSummonerByNameContext(RPCService service, string name)
            : base(service)
        {
            _name = name;
            AccountID = -1;
        }

        public override void Execute()
        {
            _service.GetSummonerByNameAsync(_name, new Responder<object>(_OnGetSummonerByName));
            _Wait();
        }

        private void _OnGetSummonerByName(object o)
        {
            if (o != null)
            {
                Dictionary<string, object> dictionary = (Dictionary<string, object>)o;
                AccountID = Convert.ToInt32(dictionary["acctId"]);
            }
            else
                AccountID = -1;

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
            _service.GetRecentGamesAsync(_accountID, new Responder<object>(_OnGetRecentGames));
            _Wait();
        }

        private void _OnGetRecentGames(object o)
        {
            Dictionary<string, object> dictionary = (Dictionary<string, object>)o;

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
                foreach (KeyValuePair<string, object> kvp in d)
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
            _service.GetAllPublicSummonerDataByAccountAsync(_accountID, new Responder<object>(_OnGetAllPublicSummonerData));
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
            _service.GetAllSummonerDataByAccountAsync(_accountID, new Responder<object>(_OnGetAllSummonerData));
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

    public class RetrievePlayerStatsByAccountIdContext : ExecutionContext
    {
        public RetrievePlayerStatsByAccountIdContext(RPCService service, int accountID, string season) : base(service)
        {
            PlayerStats = null;

            _accountID = accountID;
            _season = season;
        }

        public override void Execute()
        {
            _service.RetrievePlayerStatsByAccountIDAsync(_accountID, _season, new Responder<object>(_OnRetrievePlayerStatsByAccountID));
            _Wait();
        }

        private void _OnRetrievePlayerStatsByAccountID(object o)
        {
            PlayerStats = (Dictionary<string, object>) o;
            _Signal();
        }

        public override string ToString()
        {
            return PlayerStats.PrettyPrint();
        }

        public Dictionary<string, object> PlayerStats;
        private int _accountID;
        private string _season;
    }

    public class GetAggregatedStatsContext : ExecutionContext
    {
        public GetAggregatedStatsContext(RPCService service, int accountID, string gameMode, string season)
            : base(service)
        {
            AggregatedStats = null;

            _accountID = accountID;
            _gameMode = gameMode;
            _season = season;
        }

        public override void Execute()
        {
            _service.GetAggregatedStatsAsync(_accountID, _gameMode, _season, new Responder<object>(_OnGetAggregatedStats));
            _Wait();
        }

        private void _OnGetAggregatedStats(object o)
        {
            AggregatedStats = (Dictionary<string, object>)o;
            _Signal();
        }

        public override string ToString()
        {
            return AggregatedStats.PrettyPrint();
        }

        public Dictionary<string, object> AggregatedStats;
        private int _accountID;
        private string _gameMode; 
        private string _season;
    }

    public class GetRecentGamesByNameContext : ExecutionContext
    {

        public GetRecentGamesByNameContext(RPCService service, string name)
            : base(service)
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
}
