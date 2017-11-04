using Echo.UnitTesting;
using System.IO;
using System.Reflection;
using Echo.IntegrationTests.Subject;
using Xunit;

namespace Echo.IntegrationTests
{
    public class EndpointTests
    {
        // TODO add intentionally broken implementations, and replay echoes on them:
        // Don't call IProvider
        // Dupliacte charge call
        // Overcharge
        // No refund

        [Fact]
        public void Purchase_OriginalSource_Pass()
        {
            using (
                var resourceStream =
                    Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("Echo.IntegrationTests.Recording.HappyCase.echo"))
            {
                using (var streamReader = new StreamReader(resourceStream))
                {
                    VerifyAll_OriginalSource(streamReader);
                }
            }
        }

        private void VerifyAll_OriginalSource(StreamReader streamReader)
        {
            // Arrange

            // setup an echo player
            var reader = new EchoReader(streamReader);
            var player = new TestPlayer(reader);

            // obtain external dependencies from the player
            var billing = player.GetReplayingTarget<IBilling>();
            var provider = player.GetReplayingTarget<IProvider>();
            var testEntry = player.GetTestEntry();

            // this is the the instance that is getting tested
            // we inject external dependencies provided by the player
            var endpointUnderTest = new Endpoint(billing, provider);

            // this is an instance wrapped around test subject
            // so we can intercept and verify return value of Purchase() call
            var testEntryTarget = player.GetTestEntryTarget<IEndpoint>(endpointUnderTest);

            // Act

            // call method you'd like to test with values provided by the player
            testEntryTarget.Purchase(testEntry.GetValue<PurchaseRequest>());

            // Assert

            player.VerifyAll();
        }

        [Fact]
        public void Purchase_EndpointChargesExtra_FailsValidation()
        {
            using (
                var resourceStream =
                    Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("Echo.IntegrationTests.Recording.HappyCase.echo"))
            {
                using (var streamReader = new StreamReader(resourceStream))
                {
                    // Arrange

                    // setup an echo player
                    var reader = new EchoReader(streamReader);
                    var player = new TestPlayer(reader);

                    // obtain external dependencies from the player
                    var billing = player.GetReplayingTarget<IBilling>();
                    var provider = player.GetReplayingTarget<IProvider>();
                    var testEntry = player.GetTestEntry();

                    // this is the the instance that is getting tested
                    // we inject external dependencies provided by the player
                    var endpointUnderTest = new Endpoint_ChargesExtra(billing, provider);

                    // this is an instance wrapped around test subject
                    // so we can intercept and verify return value of Purchase() call
                    var testEntryTarget = player.GetTestEntryTarget<IEndpoint>(endpointUnderTest);

                    // Act

                    // call method you'd like to test with values provided by the player
                    testEntryTarget.Purchase(testEntry.GetValue<PurchaseRequest>());

                    // Assert

                    Assert.Throws<EchoVerificationException>(() => player.VerifyAll());
                }
            }
        }
    }
}
