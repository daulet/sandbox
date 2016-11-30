using System.Collections.Generic;

namespace Echo
{
    public interface IInvocationEntryReader
    {
        IEnumerable<string> ReadAllInvocationEntries();
    }
}
