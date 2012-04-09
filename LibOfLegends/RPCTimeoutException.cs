using System;

namespace LibOfLegends
{
	public class RPCTimeoutException : RPCException
	{
		public RPCTimeoutException(string message)
			: base(message)
		{
		}
	}
}
