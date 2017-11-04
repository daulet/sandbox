using Echo.Serialization;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Echo.Core
{
    internal class InvocationSerializer : IInvocationListener
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer(new SimpleTypeResolver());
        private readonly TextWriter _writer;

        internal InvocationSerializer(TextWriter writer)
        {
            _writer = writer;
        }

        public void WriteInvocation<TTarget>(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
            where TTarget : class
        {
            var invocationRecord = new InvocationEntry(typeof(TTarget), methodInfo, arguments, invocationResult);
            var serializedRecord = _serializer.Serialize(invocationRecord);
            _writer.WriteLine(serializedRecord);
        }
    }
}
