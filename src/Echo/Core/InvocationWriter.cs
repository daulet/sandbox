using Echo.Serialization;
using System;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Echo.Core
{
    internal class InvocationWriter : IInvocationWriter
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private readonly IEchoWriter _echoWriter;

        internal InvocationWriter(IEchoWriter echoWriter)
        {
            _echoWriter = echoWriter;
        }

        public void WriteInvocation(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
        {
            var invocationRecord = new InvocationEntry(DateTimeOffset.UtcNow, arguments, methodInfo, invocationResult);
            var serializedRecord = _serializer.Serialize(invocationRecord);
            _echoWriter.WriteSerializedInvocation(serializedRecord);
        }
    }
}
