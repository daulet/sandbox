using Echo;
using System.Collections.Generic;
using System.IO;

namespace Samples.InstantReplay
{
    internal class EntryReader : IInvocationEntryReader
    {
        private readonly byte[] _bytes;

        internal EntryReader(byte[] bytes)
        {
            _bytes = bytes;
        }

        public IEnumerable<string> ReadAllInvocationEntries()
        {
            using (var stream = new MemoryStream(_bytes))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    while (!streamReader.EndOfStream)
                    {
                        yield return streamReader.ReadLine();
                    }
                }
            }
        }
    }
}
