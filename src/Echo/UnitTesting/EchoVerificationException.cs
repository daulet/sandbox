using Echo.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Echo.UnitTesting
{
    // TODO should be two types of exceptions: external dependencies failed, or entry point result is different
    public class EchoVerificationException : Exception
    {
        private readonly IList<Invocation> _notMatchedInvocations;
        private readonly IList<Invocation> _notVisitedInvocations;

        internal EchoVerificationException(IList<Invocation> notMatchedInvocations, IList<Invocation> notVisitedInvocations)
        {
            _notMatchedInvocations = notMatchedInvocations;
            _notVisitedInvocations = notVisitedInvocations;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Extra method calls:");
            if (_notMatchedInvocations.Count > 0)
            {
                foreach (var notMatchedInvocation in _notMatchedInvocations)
                {
                    stringBuilder.AppendLine(InvocationForLogging(notMatchedInvocation));
                }
            }

            stringBuilder.AppendLine("Methods that were not called:");
            if (_notVisitedInvocations.Count > 0)
            {
                foreach (var notVisitedInvocation in _notVisitedInvocations)
                {
                    stringBuilder.AppendLine(InvocationForLogging(notVisitedInvocation));
                }
            }

            return stringBuilder.ToString();
        }

        private static string InvocationForLogging(Invocation invocation)
        {
            return $"{invocation.Target.Name}.{invocation.Method}";
        }
    }
}
