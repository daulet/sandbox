using System.Collections.Generic;

namespace Samples.Storage
{
    internal class Tape
    {
        private readonly IList<Record> _records = new List<Record>();

        public void AddRecord(Record record)
        {
            _records.Add(record);
        }
    }
}
