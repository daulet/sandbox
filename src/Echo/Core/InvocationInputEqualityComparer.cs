using Echo.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Echo.Core
{
    internal class InvocationInputEqualityComparer : IEqualityComparer<Invocation>
    {
        private readonly JTokenEqualityComparer _jsonEqualityComparer = new JTokenEqualityComparer();

        public bool Equals(Invocation x, Invocation y)
        {
            return InvocationUtility.IsArgumentListMatch(x.Arguments, y.Arguments)
                && x.Method.Equals(y.Method, StringComparison.Ordinal)
                && x.Target == y.Target;
        }

        public int GetHashCode(Invocation obj)
        {
            // 33 - the magic number
            var argumentsHash = 33;
            foreach (var argument in obj.Arguments)
            {
                argumentsHash *= _jsonEqualityComparer.GetHashCode(JToken.FromObject(argument));
            }

            return argumentsHash
                * obj.Method.GetHashCode()
                * obj.Target.GetHashCode();
        }
    }
}
