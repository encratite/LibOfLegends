using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOfLegends
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
            {"RetrievePlayerStatsByAccountID", 2},
            {"GetAggregatedStats", 3},
            {"GetRecentGamesByName", 1}
        };

        public string Name;
        public List<string> Arguments;
    }
}
