using Echo.Serialization;
using System;
using System.Reflection;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Echo.Core
{
    internal class InvocationSerializer : IInvocationListener
    {
        private readonly IEchoWriter _echoWriter;

        internal InvocationSerializer(IEchoWriter echoWriter)
        {
            _echoWriter = echoWriter;
        }

        public void WriteInvocation<TTarget>(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
            where TTarget : class
        {
            var invocationRecord = new InvocationEntry(typeof(TTarget), methodInfo, arguments, invocationResult, DateTimeOffset.UtcNow);
            var serializedRecord = JsonConvert.SerializeObject(invocationRecord);
            _echoWriter.WriteEcho(serializedRecord);
        }
    }
}
