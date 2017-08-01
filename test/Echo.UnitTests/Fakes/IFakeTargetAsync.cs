using System.Threading.Tasks;

namespace Echo.UnitTests.Fakes
{
    public interface IFakeTargetAsync
    {
        Task CallRemoteResourceAsync();

        Task<object> GetRemoteResourceAsync();
    }
}
