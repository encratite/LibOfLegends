using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOfLegendsExample
{
	delegate void CommandHandler(List<string> arguments);

	class CommandInformation
	{
		public int ArgumentCount;
		public CommandHandler Handler;
		public string ArgumentDescription;
		public string Description;

		public CommandInformation(int argumentCount, CommandHandler handler, string argumentDescription, string description)
		{
			ArgumentCount = argumentCount;
			Handler = handler;
			ArgumentDescription = argumentDescription;
			Description = description;
		}
	};
}
