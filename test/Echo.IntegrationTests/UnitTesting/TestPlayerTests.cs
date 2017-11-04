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
                    var player = new TestPlayer(streamReader);

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
                    var player = new TestPlayer(streamReader);

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

        [Theory]
        [InlineData(
            "Echo.IntegrationTests.Recording.HappyCase.echo",
            "Echo.IntegrationTests.Recording.BillingFails.echo",
            "Echo.IntegrationTests.Recording.ProvisioningFails.echo")]
        public void Purchase_EndpointChargesExtra_FailsValidation(params string[] recordings)
        {
            try
            {
                foreach (var recording in recordings)
                {
                    using (
                        var resourceStream =
                            Assembly.GetExecutingAssembly()
                                .GetManifestResourceStream(recording))
                    {
                        using (var streamReader = new StreamReader(resourceStream))
                        {
                            // Arrange

                            // setup an echo player
                            var player = new TestPlayer(streamReader);

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

                            try
                            {
                                // call method you'd like to test with values provided by the player
                                testEntryTarget.Purchase(testEntry.GetValue<PurchaseRequest>());
                            }
                            catch (PurchaseFailureException)
                            {
                                // Some tests expect the target to throw
                            }

                            player.VerifyAll();
                        }
                    }
                }
            }
            catch (EchoVerificationException)
            {
                // Assert

                Assert.True(true, "At least one recording must catch fail validation");
                return;
            }

            Assert.False(true, "Should be unreachable since at least one recording should fail validation");
        }

        [Theory]
        [InlineData(
            "Echo.IntegrationTests.Recording.HappyCase.echo",
            "Echo.IntegrationTests.Recording.BillingFails.echo",
            "Echo.IntegrationTests.Recording.ProvisioningFails.echo")]
        public void Purchase_EndpointChargesTwice_FailsValidation(params string[] recordings)
        {
            try
            {
                foreach (var recording in recordings)
                {
                    using (
                        var resourceStream =
                            Assembly.GetExecutingAssembly()
                                .GetManifestResourceStream(recording))
                    {
                        using (var streamReader = new StreamReader(resourceStream))
                        {
                            // Arrange

                            // setup an echo player
                            var player = new TestPlayer(streamReader);

                            // obtain external dependencies from the player
                            var billing = player.GetReplayingTarget<IBilling>();
                            var provider = player.GetReplayingTarget<IProvider>();
                            var testEntry = player.GetTestEntry();

                            // this is the the instance that is getting tested
                            // we inject external dependencies provided by the player
                            var endpointUnderTest = new Endpoint_DuplicateCharge(billing, provider);

                            // this is an instance wrapped around test subject
                            // so we can intercept and verify return value of Purchase() call
                            var testEntryTarget = player.GetTestEntryTarget<IEndpoint>(endpointUnderTest);

                            // Act

                            try
                            {
                                // call method you'd like to test with values provided by the player
                                testEntryTarget.Purchase(testEntry.GetValue<PurchaseRequest>());
                            }
                            catch (PurchaseFailureException)
                            {
                                // Some tests expect the target to throw
                            }

                            player.VerifyAll();
                        }
                    }
                }
            }
            catch (EchoVerificationException)
            {
                // Assert

                Assert.True(true, "At least one recording must catch fail validation");
                return;
            }

            Assert.False(true, "Should be unreachable since at least one recording should fail validation");
        }

        [Theory]
        [InlineData(
            "Echo.IntegrationTests.Recording.HappyCase.echo",
            "Echo.IntegrationTests.Recording.BillingFails.echo",
            "Echo.IntegrationTests.Recording.ProvisioningFails.echo")]
        public void Purchase_EndpointDoesNotProvision_FailsValidation(params string[] recordings)
        {
            try
            {
                foreach (var recording in recordings)
                {
                    using (
                        var resourceStream =
                            Assembly.GetExecutingAssembly()
                                .GetManifestResourceStream(recording))
                    {
                        using (var streamReader = new StreamReader(resourceStream))
                        {
                            // Arrange

                            // setup an echo player
                            var player = new TestPlayer(streamReader);

                            // obtain external dependencies from the player
                            var billing = player.GetReplayingTarget<IBilling>();
                            var provider = player.GetReplayingTarget<IProvider>();
                            var testEntry = player.GetTestEntry();

                            // this is the the instance that is getting tested
                            // we inject external dependencies provided by the player
                            var endpointUnderTest = new Endpoint_NoProvision(billing, provider);

                            // this is an instance wrapped around test subject
                            // so we can intercept and verify return value of Purchase() call
                            var testEntryTarget = player.GetTestEntryTarget<IEndpoint>(endpointUnderTest);

                            // Act

                            try
                            {
                                // call method you'd like to test with values provided by the player
                                testEntryTarget.Purchase(testEntry.GetValue<PurchaseRequest>());
                            }
                            catch (PurchaseFailureException)
                            {
                                // Some tests expect the target to throw
                            }

                            player.VerifyAll();
                        }
                    }
                }
            }
            catch (EchoVerificationException)
            {
                // Assert

                Assert.True(true, "At least one recording must catch fail validation");
                return;
            }

            Assert.False(true, "Should be unreachable since at least one recording should fail validation");
        }

        [Theory]
        [InlineData(
            "Echo.IntegrationTests.Recording.HappyCase.echo",
            "Echo.IntegrationTests.Recording.BillingFails.echo",
            "Echo.IntegrationTests.Recording.ProvisioningFails.echo")]
        public void Purchase_EndpointDoesNotRefund_FailsValidation(params string[] recordings)
        {
            try
            {
                foreach (var recording in recordings)
                {
                    using (
                        var resourceStream =
                            Assembly.GetExecutingAssembly()
                                .GetManifestResourceStream(recording))
                    {
                        using (var streamReader = new StreamReader(resourceStream))
                        {
                            // Arrange

                            // setup an echo player
                            var player = new TestPlayer(streamReader);

                            // obtain external dependencies from the player
                            var billing = player.GetReplayingTarget<IBilling>();
                            var provider = player.GetReplayingTarget<IProvider>();
                            var testEntry = player.GetTestEntry();

                            // this is the the instance that is getting tested
                            // we inject external dependencies provided by the player
                            var endpointUnderTest = new Endpoint_NoRefund(billing, provider);

                            // this is an instance wrapped around test subject
                            // so we can intercept and verify return value of Purchase() call
                            var testEntryTarget = player.GetTestEntryTarget<IEndpoint>(endpointUnderTest);

                            // Act

                            try
                            {
                                // call method you'd like to test with values provided by the player
                                testEntryTarget.Purchase(testEntry.GetValue<PurchaseRequest>());
                            }
                            catch (PurchaseFailureException)
                            {
                                // Some tests expect the target to throw
                            }

                            player.VerifyAll();
                        }
                    }
                }
            }
            catch (EchoVerificationException)
            {
                // Assert

                Assert.True(true, "At least one recording must catch fail validation");
                return;
            }

            Assert.False(true, "Should be unreachable since at least one recording should fail validation");
        }
    }
}
