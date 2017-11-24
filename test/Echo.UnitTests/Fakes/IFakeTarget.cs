namespace Echo.UnitTests.Fakes
{
    public interface IFakeTarget<T>
    {
        void CallRemoteResource();

        T GetRemoteResource();
    }
}
