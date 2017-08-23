using Echo.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;

namespace Echo.UnitTesting
{
    // TODO should be two types of exceptions: external dependencies failed, or entry point result is different
    [Serializable]
    public class EchoVerificationException : Exception
    {
        private readonly IList<Invocation> _notMatchedInvocations;
        private readonly IList<Invocation> _notVisitedInvocations;

        internal EchoVerificationException(IList<Invocation> notMatchedInvocations, IList<Invocation> notVisitedInvocations)
        {
            _notMatchedInvocations = notMatchedInvocations;
            _notVisitedInvocations = notVisitedInvocations;
        }

        // TODO improve message so that users rarely have to debug to figure out the root cause
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

                        if (matchingVisit != null)
                        {
                            stringBuilder.AppendLine(InvocationForLogging(matchingVisit));
                            var differences =
                                CompareAsJsonObjects(matchingVisit.Arguments, notMatchedInvocation.Arguments);
                            foreach (var difference in differences)
                            {
                                stringBuilder.AppendLine(
                                    $"Property: {difference.Path}, Unexpected value: {difference.Value}");
                            }

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

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("Message", Message);
        }

        private static string InvocationForLogging(Invocation invocation)
        {
            return $"{invocation.Target.Name}.{invocation.Method}";
        }

        private static IEnumerable<JProperty> CompareAsJsonObjects(IReadOnlyList<object> expectedArguments, IReadOnlyList<object> actualArguments)
        {
            // convert to JSON object
            // TODO compare more than just first argument
            var expectedObj = JObject.Parse(JsonConvert.SerializeObject(expectedArguments[0]));
            var actualObj = JObject.Parse(JsonConvert.SerializeObject(actualArguments[0]));

            // read properties
            var expectedProperties = expectedObj.Properties().ToList();
            var actualProperties = actualObj.Properties().ToList();

            // find missing properties
            // TODO compare as trees, not as a flat list of properties
            var missingProps = actualProperties
                .Where(actual => expectedProperties
                    .All(expected => expected.Value != actual.Value));

            return missingProps;
        }
    }
}
