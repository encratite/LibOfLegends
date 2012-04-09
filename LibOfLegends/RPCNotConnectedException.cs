namespace LibOfLegends
{
	public class RPCNotConnectedException : RPCException
	{
		public RPCNotConnectedException(string message)
			: base(message)
		{
		}
	}
}
