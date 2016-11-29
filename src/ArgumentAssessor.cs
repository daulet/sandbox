namespace Echo
{
    public static class ArgumentAssessor
    {
        public static bool IsMatch(object[] arguments, object[] otherArguments)
        {
            if (arguments == null || otherArguments == null)
            {
                return arguments == otherArguments;
            }
            if (arguments.Length == otherArguments.Length)
            {
                for (var argumentIndex = 0; argumentIndex < arguments.Length; argumentIndex++)
                {
                    if (!IsMatch(arguments[argumentIndex], otherArguments[argumentIndex]))
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

        public static bool IsMatch(object argument, object otherArgument)
        {
            if (argument == null || otherArgument == null)
            {
                return argument == otherArgument;
            }
            return argument.Equals(otherArgument);
        }
    }
}
