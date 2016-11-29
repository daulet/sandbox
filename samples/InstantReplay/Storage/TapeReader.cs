using Echo;
using Echo.Utilities;
using System.Collections.Generic;
using System.Reflection;

namespace Samples.InstantReplay.Storage
{
    internal class TapeReader : IInvocationReader
    {
        private readonly HashSet<Record> _records;

        public TapeReader(Tape tape)
        {
            _records = new HashSet<Record>(tape.GetRecords());
        }

        /// <summary>
        /// Match method name and arguments list - order does matter
        /// </summary>
        public object ReadReturnValue(MethodInfo methodInfo, object[] arguments)
        {
            foreach (var record in _records)
            {
                if (InvocationUtility.IsMethodMatch(record.Method, methodInfo))
                {
                    if (InvocationUtility.IsArgumentListMatch(record.Arguments, arguments))
                    {
                        _records.Remove(record);
                        return record.ReturnValue;
                    }
                }
            }
            // TODO this behaviour needs to be configurable: return null or throw
            return null;
        }
    }
}
