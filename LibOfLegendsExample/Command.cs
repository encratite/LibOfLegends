using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOfLegendsExample
{
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
            string[] parts = command.Split(new string[] { " " }, 2, StringSplitOptions.None);

            if (parts.Length == 0)
                throw new InvalidCommandException();

            Name = parts[0];

            if (!ExpectedArgs.ContainsKey(Name))
                throw new InvalidCommandException();

            Arguments = new List<string>();
            if (parts.Length > 1)
                Arguments = Utility.CommandLineToArgs(parts[1]).ToList();
            if (Arguments.Count != ExpectedArgs[Name])
                throw new InvalidCommandException();
        }

        public static Dictionary<string, int> ExpectedArgs = new Dictionary<string, int>()
        {
            {"GetSummonerByName", 1},
            {"GetRecentGames", 1},
            {"GetAllPublicSummonerDataByAccount", 1},
            {"GetAllSummonerDataByAccount", 1},
            {"RetrievePlayerStatsByAccountID", 2},
            {"GetAggregatedStats", 3},
            {"GetRecentGamesByName", 1},
            {"help", 0},
            {"?", 0}
        };

        public static Dictionary<string, string> HelpFiles = new Dictionary<string,string>()
        {
            {"GetSummonerByName", "name\n\tRetrieve the summoner data for a given name"},
            {"GetRecentGames", "accountID\n\tRetrieve a list of games for the given account ID"},
            {"GetAllPublicSummonerDataByAccount", "accountID\n\tRetrieve public summoner data for the given account ID"},
            {"GetAllSummonerDataByAccount", "accountID\n\tRetrieve summoner data for the given account ID"},
            {"RetrievePlayerStatsByAccountID", "accountID season\n\tRetrieve player stats for the given season and account ID"},
            {"GetAggregatedStats", "accountID season gameMode\n\tRetrieve aggregate and per-champion statistics for a given account ID, season and game mode"},
            {"GetRecentGamesByName", "name\n\tRetrieve a list of games for the given username"},
            {"help", "This help message"},
            {"?", "This help message"}
        };

        public static void PrintHelp()
        {
            foreach (KeyValuePair<string, string> kvp in HelpFiles)
                Console.WriteLine("{0} {1}", kvp.Key, kvp.Value);
            Console.WriteLine();
        }

        public string Name;
        public List<string> Arguments;
    }
}
