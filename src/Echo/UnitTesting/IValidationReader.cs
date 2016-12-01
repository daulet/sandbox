namespace Echo.UnitTesting
{
    internal interface IValidationReader
    {
        object[] FindEntryArguments();

        void VerifyAll();
    }
}
