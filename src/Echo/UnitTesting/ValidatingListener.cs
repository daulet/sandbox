using Echo.Core;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Echo.UnitTesting
{
    internal class ValidatingListener : IInvocationListener
    {
        private readonly IInvocationReader _invocationReader;
        private readonly IList<Invocation> _visitedInvocations;

        public ValidatingListener(IInvocationReader invocationReader)
        {
            _invocationReader = invocationReader;
            _visitedInvocations = new List<Invocation>();
        }

        public void WriteInvocation<TTarget>(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
            where TTarget : class
        {
            _visitedInvocations.Add(new Invocation(arguments, methodInfo.Name, invocationResult, typeof(TTarget)));
        }

        public void VerifyAll()
        {
            var recordedInvocations = _invocationReader.GetAllInvocations().ToList();
            var visitedInvocations = _visitedInvocations.ToList();

            // TODO test for detecting duplicate calls

            var inputComparator = new InvocationInputEqualityComparer();
            var notMatchedInvocations = visitedInvocations
                .Except(recordedInvocations, inputComparator)
                .ToList();
            var notVisitedInvocations = recordedInvocations
                .Except(visitedInvocations, inputComparator)
                .ToList();

            if (notVisitedInvocations.Any() ||
                notMatchedInvocations.Any())
            {
                throw new EchoVerificationException(
                    notMatchedInvocations: notMatchedInvocations,
                    notVisitedInvocations: notMatchedInvocations);
            }
        }

        // TODO verify method in style of Moq - the caller provides a strongly typed expression to compare certain properties only
    }
}
