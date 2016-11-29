using Echo;
using System;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Samples.RecordToFile.Storage
{
    internal class InvocationWriter : IInvocationWritter
    {
        private readonly StreamWriter _streamWriter;

        public InvocationWriter(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void RecordInvocation(MethodInfo methodInfo, object returnValue, object[] arguments)
        {
            var serializer = new JavaScriptSerializer();
            var log = new InvocationLog(DateTimeOffset.UtcNow, arguments, methodInfo, returnValue);

            _streamWriter.WriteLine(serializer.Serialize(log));
        }
    }
}
