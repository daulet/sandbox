using Echo.Serialization;
using System;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Echo.Core
{
    internal class InvocationWriter : IInvocationWritter
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private readonly IInvocationLogger _logger;

        internal InvocationWriter(IInvocationLogger logger)
        {
            _logger = logger;
        }

        public void WriteInvocation(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
        {
            var invocationRecord = new InvocationEntry(DateTimeOffset.UtcNow, arguments, methodInfo, invocationResult);
            var serializedRecord = _serializer.Serialize(invocationRecord);
            _logger.WriteSerializedInvocation(serializedRecord);
        }
    }
}
