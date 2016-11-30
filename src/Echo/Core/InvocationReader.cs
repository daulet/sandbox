using Echo.Serialization;
using Echo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Echo.Core
{
    internal class InvocationReader : IInvocationReader
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer(new SimpleTypeResolver());
        private readonly IEchoReader _echoReader;

        internal InvocationReader(IEchoReader echoReader)
        {
            _echoReader = echoReader;
        }

        public object[] FindEntryArguments()
        {
            // TODO for now just assume that last echo is the entry point
            return Echoes.Last().Arguments;
        }

        public InvocationResult FindInvocationResult<TTarget>(MethodInfo methodInfo, object[] arguments)
            where TTarget : class
        {
            var echoesCopy = new HashSet<InvocationEntry>(Echoes);

            foreach (var entry in echoesCopy)
            {
                if (string.Equals(entry.Method, methodInfo.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (InvocationUtility.IsArgumentListMatch(entry.Arguments, arguments))
                    {
                        // TODO can't remove while enumerating
                        echoesCopy.Remove(entry);

                        return entry.InvocationResult;
                    }
                }
            }
            throw new NoRecordingFoundException();
        }

        private IList<InvocationEntry> Echoes
        {
            get
            {
                if (_echoes == null)
                {
                    var serializedEntries = _echoReader.ReadAllInvocationEntries();
                    // TODO should fail more graciously if deserialization fails
                    _echoes = new List<InvocationEntry>(
                        serializedEntries.Select(x => _serializer.Deserialize<InvocationEntry>(x)));
                }
                return _echoes;
            }
        }
        private IList<InvocationEntry> _echoes;
    }
}