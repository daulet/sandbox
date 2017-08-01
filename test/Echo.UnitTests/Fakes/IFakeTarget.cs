namespace Echo.UnitTests.Fakes
{
    public interface IFakeTarget
    {
        void CallRemoteResource();

        object GetRemoteResource();
    }
}
