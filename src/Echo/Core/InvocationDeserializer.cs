using Echo.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Echo.Core
{
    internal class InvocationDeserializer : IInvocationReader
    {
        // TODO share serializer instance
        private readonly IEchoReader _echoReader;

        internal InvocationDeserializer(IEchoReader echoReader)
        {
            _echoReader = echoReader;
        }

        public object[] FindEntryArguments()
        {
            // TODO for now just assume that last echo is the entry point
            return Echoes.Last().Arguments;
        }

        public IEnumerable<Invocation> GetAllInvocations()
        {
            foreach (var entry in Echoes)
            {
                yield return new Invocation(entry.Arguments, entry.Method, entry.InvocationResult, entry.TargetType);
            }
        }

        private IEnumerable<InvocationEntry> Echoes
        {
            get
            {
                if (_echoes == null)
                {
                    var serializedEntries = _echoReader.ReadAllEchoes();
                    // TODO should fail more graciously if deserialization fails
                    _echoes = new List<InvocationEntry>(
                        serializedEntries.Select(JsonConvert.DeserializeObject<InvocationEntry>));
                }
                return _echoes;
            }
        }
        private IList<InvocationEntry> _echoes;
    }
}