using System;

namespace LibOfLegends
{
	public class TimeoutException : Exception
	{
		public TimeoutException(string message)
			: base(message)
		{
		}
	}
}
