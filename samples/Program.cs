using Echo;
using System;

namespace Samples
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var externalPartner = new ExternalPartner();

            var recorder = new Recorder();
            var interceptedPartner = recorder.GetRecordingTarget<IExternalPartner>(externalPartner);

            var result = interceptedPartner.Multiply(1, 2, 3);

            Console.WriteLine($"Received result: {result}");

            Console.ReadKey();
        }
    }
}
