using Echo.Utilities;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Echo.Core
{
    internal class InvocationInputEqualityComparer : IEqualityComparer<Invocation>
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer(new SimpleTypeResolver());

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
                argumentsHash *= _serializer.Serialize(argument).GetHashCode();
            }

            return argumentsHash
                * obj.Method.GetHashCode()
                * obj.Target.GetHashCode();
        }
    }
}
