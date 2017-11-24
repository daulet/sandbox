using Echo.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;

namespace Echo.Core
{
    internal class InvocationDeserializer : IInvocationReader
    {
        private readonly TextReader _reader;
        private readonly JsonSerializerSettings _serializerSettings;

        internal InvocationDeserializer(TextReader reader)
        {
            _reader = reader;
            _serializerSettings = new JsonSerializerSettings()
            {
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                TypeNameHandling = TypeNameHandling.Auto,
            };
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
                    var serializedEchoes = new List<string>();
                    string echo;
                    while ((echo = _reader.ReadLine()) != null)
                    {
                        serializedEchoes.Add(echo);
                    }
                    // TODO should fail more graciously if deserialization fails
                    _echoes = new List<InvocationEntry>(
                        serializedEchoes.Select(x =>
                            JsonConvert.DeserializeObject<InvocationEntry>(x, _serializerSettings)));
                }
                return _echoes;
            }
        }
        private IList<InvocationEntry> _echoes;
    }
}
