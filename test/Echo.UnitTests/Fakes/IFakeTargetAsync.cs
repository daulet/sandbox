using System.Threading.Tasks;

namespace Echo.UnitTests.Fakes
{
    public interface IFakeTargetAsync<T>
    {
        Task CallRemoteResourceAsync();

        Task<T> GetRemoteResourceAsync();
    }
}
