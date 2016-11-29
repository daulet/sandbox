using Echo;
using Samples.InstantReplay.Storage;
using System;

namespace Samples.InstantReplay
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting recording");

            // create an actual instance of external dependency
            var externalPartner = new ExternalDependency();

            // setup recording
            var tapeWritter = new TapeWritter();
            var recorder = new Recorder(tapeWritter);
            var interceptedPartner = recorder.GetRecordingTarget<IExternalDependency>(externalPartner);

            // call external dependency
            var actualResult = interceptedPartner.Multiply(1, 2, 3);

            Console.WriteLine($"Received result from actual dependency: {actualResult}");

            Console.WriteLine("Starting a replay");

            // setup replaying
            var tapeReader = new TapeReader(tapeWritter.GeTape());
            var player = new Player(tapeReader);
            var mockedPartner = player.GetReplayingTarget<IExternalDependency>();

            // call mocked dependency
            var mockedResult = mockedPartner.Multiply(1, 2, 3);

            Console.WriteLine($"Received result from mocked dependency: {mockedResult}");

            Console.ReadKey();
        }
    }
}
