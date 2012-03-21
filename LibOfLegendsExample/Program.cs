using System;

using LibOfLegends;
using Nil;

namespace LibOfLegendsExample
{
	class Program
	{
		const string ConfigurationFile = "Configuration.xml";

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
				Console.WriteLine("Unable to load configuration file \"" + ConfigurationFile + "\"");
				return;
			}
			catch (System.InvalidOperationException)
			{
				Console.WriteLine("Malformed configuration file");
				return;
			}

			if (arguments.Length != 3)
			{
				Console.WriteLine("Usage:");
				Console.WriteLine(Environment.GetCommandLineArgs()[0] + " <server> <user> <password>");
				Console.Write("Servers available:");
				foreach (ServerProfile profile in configuration.ServerProfiles)
					Console.Write(" " + profile.Abbreviation);
				Console.WriteLine("");
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
				Console.WriteLine("Unable to find server profile \"" + server + "\"");
				return;
			}

			RegionProfile regionData = new RegionProfile(chosenProfile.LoginQueueURL, chosenProfile.RPCURL);
			ConnectionProfile connectionData = new ConnectionProfile(configuration.Authentication, regionData, configuration.Proxy, user, password);

			LegendaryPrompt prompt = new LegendaryPrompt(configuration, connectionData);
			prompt.Run();
		}
	}
}
