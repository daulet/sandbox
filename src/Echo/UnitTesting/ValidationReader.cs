using Echo.Core;
using Echo.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Echo.UnitTesting
{
    // TODO shared code is minimal, subclassing can be avoided
    internal class ValidationReader : InvocationReader, IValidationReader
    {
        private readonly HashSet<TestInvocation> _extraVisits;
        private readonly HashSet<TestInvocation> _notMatchedVisits;
        // TODO can be changed to set of TestInvocation, if base matches - we can remove from Hash
        private readonly HashSet<InvocationEntry> _notVisitedEntries;

        internal ValidationReader(IEchoReader echoReader)
            : base(echoReader)
        {
            _extraVisits = new HashSet<TestInvocation>();
            _notMatchedVisits = new HashSet<TestInvocation>();
            _notVisitedEntries = new HashSet<InvocationEntry>(Echoes);
        }

        public object[] FindEntryArguments()
        {
            // TODO for now just assume that last echo is the entry point
            return Echoes.Last().Arguments;
        }

        public override InvocationResult FindInvocationResult<TTarget>(MethodInfo methodInfo, object[] arguments)
        {
            var invocationRecord = FindInvocationRecord<TTarget>(methodInfo, arguments);
            if (invocationRecord == null)
            {
                // record mismatching request
                _notMatchedVisits.Add(new TestInvocation(arguments, methodInfo, typeof(TTarget)));
                // TODO does this work for value types?
                return null;
            }

            if (_notVisitedEntries.Contains(invocationRecord))
            {
                _notVisitedEntries.Remove(invocationRecord);
            }
            else
            {
                _extraVisits.Add(new TestInvocation(arguments, methodInfo, typeof(TTarget)));
            }

            return invocationRecord.InvocationResult;
        }

        public void VerifyAll()
        {
            if (_notVisitedEntries.Count > 0)
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
