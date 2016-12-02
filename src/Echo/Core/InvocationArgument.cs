namespace Echo.Core
{
    // TODO use this to log argument name in verifitcation exception
    internal class InvocationArgument
    {
        public string ArgumentName { get; }

        public object ArgumentValue { get; }

        public InvocationArgument(string argumentName, object argumentValue)
        {
            ArgumentName = argumentName;
            ArgumentValue = argumentValue;
        }
    }
}
