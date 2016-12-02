using Echo.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Samples.Demo.Source;
using System.IO;
using System.Reflection;

namespace Samples.Demo.UnitTests
{
    [TestClass]
    public class EndpointTests
    {
        [TestMethod]
        public void Purchase_HappyCase_ReturnsResponse()
        {
            using (var resourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("Samples.Demo.UnitTests.Echoes.HappyCase.echo"))
            {
                using (var streamReader = new StreamReader(resourceStream))
                {
                    // Arrange

                    // setup an echo player
                    var reader = new EchoReader(streamReader);
                    var player = new TestPlayer(reader);

                    // obtain replayable instances from the player
                    var billing = player.GetReplayingTarget<IBilling>();
                    var serviceProvider = player.GetReplayingTarget<IProvider>();
                    var endpointUnderTest = new Endpoint(billing, serviceProvider);
                    var testEntryTarget = player.GetTestEntryTarget<IEndpoint>(endpointUnderTest);

                    // Act

                    var testEntry = player.GetTestEntry();
                    testEntryTarget.Purchase(testEntry.GetValue<PurchaseRequest>());

                    // Assert

                    player.VerifyAll();
                }
            }
        }
    }
}
