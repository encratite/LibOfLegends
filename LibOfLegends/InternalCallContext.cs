using System;
using System.Threading;

using FluorineFx.Net;

namespace LibOfLegends
{
	delegate void InternalCallType<ReturnType>(Responder<ReturnType> responder, object[] arguments);

	class InternalCallContext<ReturnType>
	{
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
			ReturnEvent.WaitOne();
			if (CallFault != null)
			{
				//An error occurred, throw an exception
				throw new Exception(CallFault.FaultString);
			}
			return Result;
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
