using Echo;
using System.Collections.Generic;
using System.IO;

namespace Samples.InstantReplay
{
    internal class EchoReader : IEchoReader
    {
        private readonly byte[] _bytes;

        internal EchoReader(byte[] bytes)
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
