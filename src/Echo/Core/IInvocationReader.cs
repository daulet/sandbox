using System.Collections.Generic;

namespace Echo.Core
{
    public interface IInvocationReader
    {
        object[] FindEntryArguments();

        IEnumerable<Invocation> GetAllInvocations();
    }
}
