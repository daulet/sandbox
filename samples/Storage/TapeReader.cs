using Echo;
using System.Collections.Generic;
using System.Reflection;

namespace Samples.Storage
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
                if (record.Method.Equals(methodInfo))
                {
                    if (ArgumentAssessor.IsMatch(record.Arguments, arguments))
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
