namespace Samples.InstantReplay
{
    public interface IExternalPartner
    {
        int Multiply(int a, int b, int c);
    }

    public class ExternalPartner : IExternalPartner
    {
        public int Multiply(int a, int b, int c)
        {
            return a * b * c;
        }
    }
}
