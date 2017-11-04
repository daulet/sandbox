using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Echo.Utilities
{
    // @TODO remove static
    // @TODO make internal and add more rigorous tests
    public static class InvocationUtility
    {
        public static bool IsArgumentListMatch(object[] arguments, object[] otherArguments)
        {
            if (arguments == null || otherArguments == null)
            {
                return arguments == otherArguments;
            }
            if (arguments.Length == otherArguments.Length)
            {
                for (var argumentIndex = 0; argumentIndex < arguments.Length; argumentIndex++)
                {
                    if (!IsArgumentMatch(arguments[argumentIndex], otherArguments[argumentIndex]))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsArgumentMatch(object argument, object otherArgument)
        {
            if (argument == null || otherArgument == null)
            {
                return argument == otherArgument;
            }

            if (argument.GetType() != otherArgument.GetType())
            {
                return false;
            }

            if (argument.GetType().IsValueType)
            {
                return argument.Equals(otherArgument);
            }

            return JToken.DeepEquals(
                JObject.FromObject(argument),
                JObject.FromObject(otherArgument));
        }
    }
}
