using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public InternalCallContext(InternalCallType<ReturnType> internalCall, object[] arguments)
		{
			InternalCall = internalCall;
			Arguments = arguments;

			ReturnEvent = new AutoResetEvent(false);
		}

		public ReturnType Execute()
		{
			InternalCall(new Responder<ReturnType>(OnReturn), Arguments);
			ReturnEvent.WaitOne();
			return Result;
		}

		void OnReturn(ReturnType result)
		{
			Result = result;
			ReturnEvent.Set();
		}
	}
}
