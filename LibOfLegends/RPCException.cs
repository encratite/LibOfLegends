using System;

namespace LibOfLegends
{
	public class RPCException : Exception
	{
		public RPCException(string message)
			: base(message)
		{
		}
	}
}
