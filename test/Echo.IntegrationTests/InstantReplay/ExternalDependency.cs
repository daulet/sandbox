namespace Echo.IntegrationTests.InstantReplay
{
    public interface IExternalDependency
    {
        int Multiply(int a, int b, int c);
    }

    public class ExternalDependency : IExternalDependency
    {
        public int Multiply(int a, int b, int c)
        {
            return a * b * c;
        }
    }
}
