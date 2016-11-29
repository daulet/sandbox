namespace Echo.UnitTests.Fakes
{
    internal class FakeDependency : IFakeDependency
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
