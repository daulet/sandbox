using System.Reflection;
using Newtonsoft.Json;

namespace Echo.Utilities
{
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
            // TODO find a way to differentiate multiple calls to the same method - they'll obviously have matching list of argument types
            // perhaps compare them in serialized version. You were right :)
            otherArgument = JsonConvert.SerializeObject(otherArgument);
            argument = JsonConvert.SerializeObject(argument);
            return argument.GetType() == otherArgument.GetType();
        }

        public static bool IsMethodMatch(MethodInfo method, MethodInfo anotherMethod)
        {
            if (method == anotherMethod)
            {
                return method == anotherMethod;
            }
            return method.Equals(anotherMethod);
        }
    }
}
