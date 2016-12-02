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
        public void Purchase_DependenciesSucceed_PurchaseSucceeds()
        {
            using (var resourceStream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Samples.Demo.UnitTests.Echoes.HappyCase.echo"))
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

        [TestMethod]
        public void Purchase_BillingFails_ProvisioningIsNotCalled()
        {
            using (var resourceStream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Samples.Demo.UnitTests.Echoes.BillingFails.echo"))
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

                    try
                    {
                        var testEntry = player.GetTestEntry();
                        testEntryTarget.Purchase(testEntry.GetValue<PurchaseRequest>());
                    }
                    catch (PurchaseFailureException)
                    {
                        // the Purchase is expected to fail since IProvider failed
                    }

                    // Assert

                    player.VerifyAll();
                }
            }
        }

        [TestMethod]
        public void Purchase_ProvisioningFails_RefundIsMade()
        {
            using (var resourceStream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Samples.Demo.UnitTests.Echoes.ProvisioningFails.echo"))
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

                    try
                    {
                        var testEntry = player.GetTestEntry();
                        testEntryTarget.Purchase(testEntry.GetValue<PurchaseRequest>());
                    }
                    catch (PurchaseFailureException)
                    {
                        // the Purchase is expected to fail since IProvider failed
                    }

                    // Assert

                    player.VerifyAll();
                }
            }
        }
    }
}
