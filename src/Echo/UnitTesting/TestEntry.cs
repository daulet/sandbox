namespace Echo.UnitTesting
{
    public class TestEntry
    {
        private readonly object[] _arguments;
        private int _currentArgumentIndex;

        internal TestEntry(object[] arguments)
        {
            _arguments = arguments;
            _currentArgumentIndex = 0;
        }

        public T GetValue<T>()
        {
            // Return arguments in order so the caller doesn't have to specify index of each argument.
            // Implicit, but simplifies usage for the user, especially if argument list contains same types.
            return (T)_arguments[_currentArgumentIndex++];
        }
    }
}
