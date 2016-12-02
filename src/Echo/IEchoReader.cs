using System.Collections.Generic;

namespace Echo
{
    public interface IEchoReader : IFluentInterface
    {
        IEnumerable<string> ReadAllEchoes();
    }
}
