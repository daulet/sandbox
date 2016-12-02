using System.Collections.Generic;

namespace Echo
{
    public interface IEchoReader
    {
        IEnumerable<string> ReadAllEchoes();
    }
}
