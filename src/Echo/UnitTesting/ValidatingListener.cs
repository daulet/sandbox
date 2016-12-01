using Echo.Core;
using System.Collections.Generic;
using System.Reflection;

namespace Echo.UnitTesting
{
    internal class ValidatingListener : IInvocationListener
    {
        private readonly IInvocationReader _invocationReader;

        private readonly HashSet<Invocation> _extraVisits;
        private readonly HashSet<Invocation> _notMatchedVisits;
        private readonly HashSet<Invocation> _visitedInvocations;

        public ValidatingListener(IInvocationReader invocationReader)
        {
            _invocationReader = invocationReader;

            _extraVisits = new HashSet<Invocation>();
            _notMatchedVisits = new HashSet<Invocation>();
            _visitedInvocations = new HashSet<Invocation>();
        }

        public void WriteInvocation<TTarget>(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
            where TTarget : class
        {
            _visitedInvocations.Add(new Invocation(arguments, methodInfo.Name, invocationResult, typeof(TTarget)));
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
