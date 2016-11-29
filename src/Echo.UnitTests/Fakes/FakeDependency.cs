namespace Echo.UnitTests.Fakes
{
    internal class FakeDependency : IFakeDependency
    {
        public object GetRemoteResource()
        {
            return new object();
        }
    }
}
