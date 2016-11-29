namespace Echo.UnitTests.Fakes
{
    internal class FakeTarget : IFakeTarget
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
