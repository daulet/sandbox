using Echo.UnitTesting;
using Samples.Demo.Source;
using Xunit;

namespace Samples.Demo.UnitTests
{
    public class EndpointTests
    {
        [Theory, ClassData(typeof(PurchaseSucceedsTestData))]
        public void Purchase_Succeeds_DoesNotThrow(EchoReader reader)
        {
            // Arrange

            // setup an echo player
            var player = new TestPlayer(reader);

            // obtain replayable instances from the player
            var billing = player.GetReplayingTarget<IBilling>();
            var provider = player.GetReplayingTarget<IProvider>();
            var endpointUnderTest = new Endpoint(billing, provider);
            var testEntryTarget = player.GetTestEntryTarget<IEndpoint>(endpointUnderTest);

            // Act

            var testEntry = player.GetTestEntry();
            testEntryTarget.Purchase(testEntry.GetValue<PurchaseRequest>());

            // Assert

            player.VerifyAll();
        }

        [Theory, ClassData(typeof(PurchaseFailsTestData))]
        public void Purchase_Fails_ThrowsPurchaseFailureException(EchoReader reader)
        {
            // Arrange

            // setup an echo player
            var player = new TestPlayer(reader);

            // obtain replayable instances from the player
            var billing = player.GetReplayingTarget<IBilling>();
            var provider = player.GetReplayingTarget<IProvider>();
            var endpointUnderTest = new Endpoint(billing, provider);
            var testEntryTarget = player.GetTestEntryTarget<IEndpoint>(endpointUnderTest);

            // Act

            var testEntry = player.GetTestEntry();
            Assert.Throws<PurchaseFailureException>(() =>
                testEntryTarget.Purchase(testEntry.GetValue<PurchaseRequest>()));

            // Assert

            player.VerifyAll();
        }
    }
}
