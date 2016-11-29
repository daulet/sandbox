using Echo;
using Samples.Storage;
using System;

namespace Samples
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // create an actual instance of external dependency
            var externalPartner = new ExternalPartner();

            // setup recording
            var tapeRecorder = new TapeWritter();
            var recorder = new Recorder(tapeRecorder);
            var interceptedPartner = recorder.GetRecordingTarget<IExternalPartner>(externalPartner);

            // call external dependency
            var result = interceptedPartner.Multiply(1, 2, 3);



            Console.WriteLine($"Received result: {result}");
            Console.ReadKey();
        }
    }
}
