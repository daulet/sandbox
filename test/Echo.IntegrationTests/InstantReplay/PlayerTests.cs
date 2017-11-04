using System.IO;
using Xunit;

namespace Echo.IntegrationTests.InstantReplay
{
    public class PlayerTests
    {
        [Fact]
        public void GetReplayingTarget_TestSameInput_MatchingResponseReturned()
        {
            byte[] recording;
            int expectedReturnValue;

            // Record

            using (var stream = new MemoryStream())
            {
                // create an actual instance of external dependency
                var externalPartner = new ExternalDependency();

                using (var streamWriter = new StreamWriter(stream))
                {
                    // setup recording
                    var writer = new EchoWriter(streamWriter);
                    var recorder = new Recorder(writer);
                    var interceptedPartner = recorder.GetRecordingTarget<IExternalDependency>(externalPartner);

                    // call external dependency
                    expectedReturnValue = interceptedPartner.Multiply(1, 2, 3);
                }

                recording = stream.ToArray();
            }

            // Replay

            using (var stream = new MemoryStream(recording))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    // NOTE replay logic doesn't use ExternalDependency implementation

                    // setup replaying
                    var reader = new EchoReader(streamReader);
                    var player = new Player(reader);
                    var mockedPartner = player.GetReplayingTarget<IExternalDependency>();

                    // call mocked dependency
                    var mockedResult = mockedPartner.Multiply(1, 2, 3);

                    Assert.Equal(expectedReturnValue, mockedResult);
                    Assert.NotEqual(0, mockedResult);
                }
            }
        }

        [Fact]
        public void GetReplayingTarget_TestDifferentInput_DefaultValueReturned()
        {
            byte[] recording;

            // Record

            using (var stream = new MemoryStream())
            {
                // create an actual instance of external dependency
                var externalPartner = new ExternalDependency();

                using (var streamWriter = new StreamWriter(stream))
                {
                    // setup recording
                    var writer = new EchoWriter(streamWriter);
                    var recorder = new Recorder(writer);
                    var interceptedPartner = recorder.GetRecordingTarget<IExternalDependency>(externalPartner);

                    // call external dependency
                    interceptedPartner.Multiply(4, 5, 6);
                }

                recording = stream.ToArray();
            }

            // Replay

            using (var stream = new MemoryStream(recording))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    // NOTE replay logic doesn't use ExternalDependency implementation

                    // setup replaying
                    var reader = new EchoReader(streamReader);
                    var player = new Player(reader);
                    var mockedPartner = player.GetReplayingTarget<IExternalDependency>();

                    // call mocked dependency
                    var mockedResult = mockedPartner.Multiply(7, 8, 9);

                    // expecting a default value when no matching recording found
                    Assert.Equal(0, mockedResult);
                }
            }
        }
    }
}
