using Echo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

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

        public override string Message
        {
            get
            {
                if (_message == null)
                {
                    var serializer = new JavaScriptSerializer();
                    var stringBuilder = new StringBuilder();

                    var notMatchedInvocations = _notMatchedInvocations.ToList();
                    var notVisitedInvocations = _notVisitedInvocations.ToList();

                    foreach (var notMatchedInvocation in notMatchedInvocations.ToList())
                    {
                        var matchingVisit = notVisitedInvocations.Find(x =>
                            x.Target == notMatchedInvocation.Target &&
                            x.Method == notMatchedInvocation.Method);

                        // TODO match by method name

                        if (matchingVisit != null)
                        {
                            stringBuilder.AppendLine(InvocationForLogging(matchingVisit));
                            stringBuilder.AppendLine($"Expected: {serializer.Serialize(matchingVisit.Arguments)}; " +
                                                     $"Actual: {serializer.Serialize(notMatchedInvocation.Arguments)}");

                            notMatchedInvocations.Remove(notMatchedInvocation);
                            notVisitedInvocations.Remove(matchingVisit);
                        }
                    }

                    if (notMatchedInvocations.Count > 0)
                    {
                        stringBuilder.AppendLine("Extra method calls:");
                        foreach (var notMatchedInvocation in notMatchedInvocations)
                        {
                            stringBuilder.AppendLine(InvocationForLogging(notMatchedInvocation));
                        }
                    }

                    if (notVisitedInvocations.Count > 0)
                    {
                        stringBuilder.AppendLine("Methods that were not called:");
                        foreach (var notVisitedInvocation in notVisitedInvocations)
                        {
                            stringBuilder.AppendLine(InvocationForLogging(notVisitedInvocation));
                        }
                    }

                    _message = stringBuilder.ToString();
                }
                return _message;
            }
        }
        private string _message;

        private static string InvocationForLogging(Invocation invocation)
        {
            return $"{invocation.Target.Name}.{invocation.Method}";
        }
    }
}
