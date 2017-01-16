using Echo;
using System.Collections.Generic;
using System.IO;

namespace Samples.Demo.UnitTests
{
    public class EchoReader : IEchoReader
    {
        private readonly IList<string> _echoes;
        private int _lastReadIndex;

        internal EchoReader(IList<string> echoes)
        {
            _echoes = echoes;
            _lastReadIndex = 0;
        }

        public string ReadLine()
        {
            if (_lastReadIndex < _echoes.Count)
            {
                return _echoes[_lastReadIndex++];
            }
            return null;
        }
    }
}
