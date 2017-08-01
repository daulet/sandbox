using Echo;
using System;
using System.Collections.Generic;

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
            IList<string> echos = new List<string>();
            var writer = new EchoWriter(echos);
            var recorder = new Recorder(writer);
            var interceptedPartner = recorder.GetRecordingTarget<IExternalDependency>(externalPartner);

            // call external dependency
            var actualResult = interceptedPartner.Multiply(1, 2, 3);

            Console.WriteLine($"Received result from actual dependency: {actualResult}");

            Console.WriteLine("Starting a replay");

            // setup replaying
            var reader = new EchoReader(echos);
            var player = new Player(reader);
            var mockedPartner = player.GetReplayingTarget<IExternalDependency>();

            // call mocked dependency
            var mockedResult = mockedPartner.Multiply(1, 2, 3);

            Console.WriteLine($"Received result from mocked dependency: {mockedResult}");

            Console.ReadKey();
        }
    }
}
