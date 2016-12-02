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

            if (notMatchedInvocations.Any() ||
                notVisitedInvocations.Any())
            {
                throw new EchoVerificationException(
                    notMatchedInvocations: notMatchedInvocations,
                    notVisitedInvocations: notVisitedInvocations);
            }

            // this can only happen if there are duplicate calls with exactly the same parameters
            // TODO simplify! (written last minute before demo)
            if (recordedInvocations.Count != visitedInvocations.Count)
            {
                var smallerList = recordedInvocations.Count > visitedInvocations.Count
                    ? visitedInvocations
                    : recordedInvocations;

                var biggerList = recordedInvocations.Count > visitedInvocations.Count
                    ? recordedInvocations
                    : visitedInvocations;

                var mutableBiggerList = new List<Invocation>(biggerList);
                var mutableSmallerList = new List<Invocation>(smallerList);

                foreach (var item in smallerList)
                {
                    var bigItemToRemove = mutableBiggerList.Find(x => inputComparator.Equals(x, item));
                    mutableBiggerList.Remove(bigItemToRemove);

                    var smallItemToRemove = mutableSmallerList.Find(x => inputComparator.Equals(x, item));
                    mutableSmallerList.Remove(smallItemToRemove);
                }

                throw new EchoVerificationException(
                    notMatchedInvocations: mutableBiggerList,
                    notVisitedInvocations: mutableSmallerList);
            }
        }

        // TODO verify method in style of Moq - the caller provides a strongly typed expression to compare certain properties only
    }
}
