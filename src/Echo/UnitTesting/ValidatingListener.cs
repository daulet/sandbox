using Echo.Core;
using System.Collections.Generic;
using System.Reflection;

namespace Echo.UnitTesting
{
    internal class ValidatingListener : IInvocationListener
    {
        private readonly IInvocationReader _invocationReader;

        private readonly HashSet<TestInvocation> _extraVisits;
        private readonly HashSet<TestInvocation> _notMatchedVisits;
        private readonly HashSet<TestInvocation> _visitedInvocations;

        public ValidatingListener(IInvocationReader invocationReader)
        {
            _invocationReader = invocationReader;

            _extraVisits = new HashSet<TestInvocation>();
            _notMatchedVisits = new HashSet<TestInvocation>();
            _visitedInvocations = new HashSet<TestInvocation>();
        }

        public void WriteInvocation<TTarget>(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
            where TTarget : class
        {
            _visitedInvocations.Add(new TestInvocation(arguments, methodInfo, invocationResult, typeof(TTarget)));
        }

        public void VerifyAll()
        {
            if (_visitedInvocations.Count > 0)
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
