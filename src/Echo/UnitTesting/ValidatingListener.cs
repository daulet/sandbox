using Echo.Core;
using System.Collections.Generic;
using System.Reflection;

namespace Echo.UnitTesting
{
    internal class ValidatingListener : IInvocationListener
    {
        private readonly HashSet<TestInvocation> _extraVisits;
        private readonly HashSet<TestInvocation> _notMatchedVisits;
        private readonly HashSet<TestInvocation> _visitedEntries;

        public ValidatingListener()
        {
            _extraVisits = new HashSet<TestInvocation>();
            _notMatchedVisits = new HashSet<TestInvocation>();
            _visitedEntries = new HashSet<TestInvocation>();
        }

        public void WriteInvocation<TTarget>(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
            where TTarget : class
        {
            // TODO not recording events here
        }

        public void VerifyAll()
        {
            if (_visitedEntries.Count > 0)
            {
                throw new ValidationFailedException();
            }

            if (_notMatchedVisits.Count > 0)
            {
                throw new ValidationFailedException();
            }

            if (_extraVisits.Count > 0)
            {
                throw new ValidationFailedException();
            }
        }
    }
}
