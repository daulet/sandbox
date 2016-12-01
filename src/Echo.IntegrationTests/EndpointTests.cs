using Echo.IntegrationTests.Source;
using Echo.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace Echo.IntegrationTests
{
    [TestClass]
    public class EndpointTests
    {
        // TODO add intentionally broken implementations, and replay echoes on them

        [TestMethod]
        public void Purchase_OriginalSource_Pass()
        {
            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Echo.IntegrationTests.Echoes.output1.echo"))
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
            var serviceProvider = player.GetReplayingTarget<IProvider>();
            var testEntry = player.GetTestEntry();

            // this is the the instance that is getting tested
            // we inject external dependencies provided by the player
            var endpointUnderTest = new Endpoint(billing, serviceProvider);

            // this is an instance wrapped around test subject
            // so we can intercept and verify return value of Purchase() call
            var testEntryTarget = player.GetTestEntryTarget<IEndpoint>(endpointUnderTest);

            // Act

            // call method you'd like to test with values provided by the player
            testEntryTarget.Purchase(testEntry.GetValue<PurchaseRequest>());

            // Assert

            player.VerifyAll();
        }

        private void VerifyMocks_OriginalSource(StreamReader streamReader)
        {
            // Arrange

            // setup an echo player
            var reader = new EchoReader(streamReader);
            var player = new TestPlayer(reader);

            // obtain external dependencies from the player
            var billing = player.GetReplayingTarget<IBilling>();
            var serviceProvider = player.GetReplayingTarget<IProvider>();
            var testEntry = player.GetTestEntry();

            // this is the the instance that is getting tested
            // we inject external dependencies provided by the player
            var endpointUnderTest = new Endpoint(billing, serviceProvider);

            // Act

            // call method you'd like to test with values provided by the player
            endpointUnderTest.Purchase(testEntry.GetValue<PurchaseRequest>());

            // Assert

            player.VerifyAll();
        }
    }
}
