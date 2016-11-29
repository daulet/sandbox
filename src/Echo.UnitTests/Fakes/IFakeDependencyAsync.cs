using System.Threading.Tasks;

namespace Echo.UnitTests.Fakes
{
    public interface IFakeDependencyAsync
    {
        Task CallRemoteResourceAsync();

        Task<object> GetRemoteResourceAsync();
    }
}
