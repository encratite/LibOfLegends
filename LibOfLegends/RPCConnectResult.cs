using System;
using System.Collections.Generic;

using FluorineFx.Net;

namespace LibOfLegends
{
	public enum RPCConnectResultType
	{
		Success,
		AuthenticationError,
		FlexError,
		LoginFault,
		FlexLoginFault,
	};

	public class RPCConnectResult
	{
		public readonly RPCConnectResultType Result;
		public readonly Exception ConnectionError;
		public readonly string FlexErrorMessage;
		public readonly Fault FlexLoginFault;

		public RPCConnectResult(Exception exception)
		{
			Result = RPCConnectResultType.AuthenticationError;
			ConnectionError = exception;
		}

		public RPCConnectResult(string flexMessage)
		{
			if (flexMessage == "success")
				Result = RPCConnectResultType.Success;
			else
			{
				Result = RPCConnectResultType.FlexError;
				FlexErrorMessage = flexMessage;
			}
		}

		public RPCConnectResult(RPCConnectResultType result, Fault fault)
		{
			Result = result;
			FlexLoginFault = fault;
		}

		public string GetMessage()
		{
			switch (Result)
			{
				case RPCConnectResultType.Success:
					return "Successfully connected to the server";
				case RPCConnectResultType.AuthenticationError:
					return "Authentication error: " + ConnectionError.Message;
				case RPCConnectResultType.FlexError:
					return "RPC error: " + FlexErrorMessage;
				case RPCConnectResultType.LoginFault:
				case RPCConnectResultType.FlexLoginFault:
					return "Flex login error: " + FlexLoginFault.FaultString;
			}
			return "Unknown condition";
		}

		public bool Success()
		{
			return Result == RPCConnectResultType.Success;
		}
	}
}
