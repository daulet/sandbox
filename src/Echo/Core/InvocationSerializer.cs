using Echo.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters;

namespace Echo.Core
{
    internal class InvocationSerializer : IInvocationListener
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly TextWriter _writer;

        internal InvocationSerializer(TextWriter writer)
        {
            _serializerSettings = new JsonSerializerSettings()
            {
                Converters = { new PrimitiveJsonConverter() },
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                TypeNameHandling = TypeNameHandling.All
            };
            _writer = writer;
        }

        public void WriteInvocation<TTarget>(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
            where TTarget : class
        {
            var invocationRecord = new InvocationEntry(typeof(TTarget), methodInfo, arguments, invocationResult);
            var serializedRecord = JsonConvert.SerializeObject(invocationRecord, _serializerSettings);
            _writer.WriteLine(serializedRecord);
        }
    }
}
