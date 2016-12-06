using Echo;
using System.Collections.Generic;
using System.IO;

namespace Samples.Demo.UnitTests
{
    public class EchoReader : IEchoReader
    {
        private readonly IList<string> _echoes;

        internal EchoReader(IList<string> echoes)
        {
            _echoes = echoes;
        }

        public IEnumerable<string> ReadAllEchoes()
        {
            return _echoes;
        }
    }
}
