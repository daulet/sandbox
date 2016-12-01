using System.Collections.Generic;

namespace Echo.Core
{
    internal interface IInvocationReader
    {
        object[] FindEntryArguments();

        IEnumerable<Invocation> GetAllInvocations();
    }
}
