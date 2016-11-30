using Echo;
using System;
using System.IO;

namespace Samples.InstantReplay
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting recording");

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    // create an actual instance of external dependency
                    var externalPartner = new ExternalDependency();

                    // setup recording
                    var writter = new EntityWritter(streamWriter);
                    var recorder = new Recorder(writter);
                    var interceptedPartner = recorder.GetRecordingTarget<IExternalDependency>(externalPartner);

                    // call external dependency
                    var actualResult = interceptedPartner.Multiply(1, 2, 3);

                    Console.WriteLine($"Received result from actual dependency: {actualResult}");
                }

                bytes = stream.ToArray();
            }

            Console.WriteLine("Starting a replay");

            // setup replaying
            var reader = new EntryReader(bytes);
            var player = new Player(reader);
            var mockedPartner = player.GetReplayingTarget<IExternalDependency>();

            // call mocked dependency
            var mockedResult = mockedPartner.Multiply(1, 2, 3);

            Console.WriteLine($"Received result from mocked dependency: {mockedResult}");

            Console.ReadKey();
        }
    }
}
