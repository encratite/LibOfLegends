using System;
using System.Threading;

using FluorineFx.Net;

namespace LibOfLegends
{
	delegate void InternalCallType<ReturnType>(Responder<ReturnType> responder, object[] arguments);

	class InternalCallContext<ReturnType>
	{
		//There is a timeout for every call to prevent it from blocking indefinitely
		//An exception is raised when it occurs
		const int Timeout = 10000;

		InternalCallType<ReturnType> InternalCall;
		object[] Arguments;
		AutoResetEvent ReturnEvent;
		ReturnType Result;
		Fault CallFault;

		public InternalCallContext(InternalCallType<ReturnType> internalCall, object[] arguments)
		{
			InternalCall = internalCall;
			Arguments = arguments;
			CallFault = null;

			ReturnEvent = new AutoResetEvent(false);
		}

		public ReturnType Execute()
		{
			InternalCall(new Responder<ReturnType>(OnReturn, OnFault), Arguments);
			if (ReturnEvent.WaitOne(Timeout))
			{
				if (CallFault != null)
				{
					//An error occurred, throw an exception
					throw new RPCException(CallFault.FaultString);
				}
				else
					return Result;
			}
			else
				throw new RPCTimeoutException("RPC timeout");
		}

		void OnFault(Fault fault)
		{
			CallFault = fault;
			ReturnEvent.Set();
		}

		void OnReturn(ReturnType result)
		{
			Result = result;
			ReturnEvent.Set();
		}
	}
}
