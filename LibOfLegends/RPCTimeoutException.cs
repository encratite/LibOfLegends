using System;

namespace LibOfLegends
{
	public class RPCTimeoutException : Exception
	{
		public RPCTimeoutException(string message)
			: base(message)
		{
		}
	}
}
