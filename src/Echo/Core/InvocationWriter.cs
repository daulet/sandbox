using Echo.Serialization;
using System;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Echo.Core
{
    internal class InvocationWriter : IInvocationWriter
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer(new SimpleTypeResolver());
        private readonly IEchoWriter _echoWriter;

        internal InvocationWriter(IEchoWriter echoWriter)
        {
            _echoWriter = echoWriter;
        }

        public void WriteInvocation<TTarget>(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
            where TTarget : class
        {
            var invocationRecord = new InvocationEntry(typeof(TTarget), methodInfo, arguments, invocationResult, DateTimeOffset.UtcNow);
            var serializedRecord = _serializer.Serialize(invocationRecord);
            _echoWriter.WriteSerializedInvocation(serializedRecord);
        }
    }
}
