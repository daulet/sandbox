using Echo;
using System.Collections.Generic;
using System.IO;

namespace Samples.MultiDependency
{
    internal class EchoReader : IEchoReader
    {
        private readonly StreamReader _streamReader;

        internal EchoReader(StreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        public IEnumerable<string> ReadAllEchoes()
        {
            while (!_streamReader.EndOfStream)
            {
                yield return _streamReader.ReadLine();
            }
        }
    }
}
