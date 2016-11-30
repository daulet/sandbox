using Echo;
using System.Collections.Generic;

namespace Samples.MultiDependency
{
    internal class EchoReader : IEchoReader
    {
        private readonly IList<string> _records;

        internal EchoReader(IList<string> records)
        {
            _records = records;
        }

        public IEnumerable<string> ReadAllInvocationEntries()
        {
            return new List<string>(_records);
        }
    }
}
