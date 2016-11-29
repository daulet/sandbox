namespace Echo.UnitTests.Fakes
{
    public interface IFakeDependency
    {
        void CallRemoteResource();

        object GetRemoteResource();
    }
}
