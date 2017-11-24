namespace Echo.UnitTests.Fakes
{
    internal class FakeTarget : IFakeTarget<object>
    {
        public void CallRemoteResource()
        {
        }

        public object GetRemoteResource()
        {
            return new object();
        }
    }
}
