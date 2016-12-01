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

        public virtual InvocationResult FindInvocationResult<TTarget>(MethodInfo methodInfo, object[] arguments)
            where TTarget : class
        {
            var invocationRecord = FindInvocationRecord<TTarget>(methodInfo, arguments);
            if (invocationRecord == null)
            {
                throw new NoEchoFoundException();
            }
            return invocationRecord.InvocationResult;
        }

        protected InvocationEntry FindInvocationRecord<TTarget>(MethodInfo methodInfo, object[] arguments)
            where TTarget : class
        {
            foreach (var entry in Echoes)
            {
                if (string.Equals(entry.Method, methodInfo.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (InvocationUtility.IsArgumentListMatch(entry.Arguments, arguments))
                    {
                        return entry;
                    }
                }
            }
            return null;
        }

        protected IList<InvocationEntry> Echoes
        {
            get
            {
                if (_echoes == null)
                {
                    var serializedEntries = _echoReader.ReadAllEchoes();
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