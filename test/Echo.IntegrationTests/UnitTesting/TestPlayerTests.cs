using System.IO;
using System.Reflection;
using Echo.IntegrationTests.Subject;
using Echo.UnitTesting;
using Xunit;

namespace Echo.IntegrationTests.UnitTesting
{
    public class TestPlayerTests
    {
        [Theory]
        [InlineData("Echo.IntegrationTests.Recording.HappyCase.echo")]
        public void Purchase_Succeeds_DoesNotThrow(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var streamReader =new StreamReader(stream))
                {
                    // Arrange

                    // setup an echo player
                    var reader = new EchoReader(streamReader);
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
            }
        }

        [Theory]
        [InlineData("Echo.IntegrationTests.Recording.BillingFails.echo")]
        [InlineData("Echo.IntegrationTests.Recording.ProvisioningFails.echo")]
        public void Purchase_Fails_ThrowsPurchaseFailureException(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    // Arrange

                    // setup an echo player
                    var reader = new EchoReader(streamReader);
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
    }
}
