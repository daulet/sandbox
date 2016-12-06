using Echo.UnitTesting;
using Samples.Demo.Source;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;
using System;
using System.Collections;

namespace Samples.Demo.UnitTests
{
    public class EndpointTests
    {
        [Theory, ClassData(typeof(SuccessfulTestData))]
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

        [Theory, ClassData(typeof(FailingTestData))]
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

        private class SuccessfulTestData : IEnumerable<object[]>
        {
            private readonly IList<string> _testCaseFilenames = new List<string>
            {
                "Samples.Demo.UnitTests.Echoes.HappyCase.echo",
            };
            
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach(var testCaseFilename in _testCaseFilenames)
                {
                    using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(testCaseFilename))
                    {
                        using (var streamReader = new StreamReader(resourceStream))
                        {
                            var echoes = new List<string>();
                            while(!streamReader.EndOfStream)
                            {
                                echoes.Add(streamReader.ReadLine());
                            }
                            yield return new object[] { new EchoReader(echoes) };
                        }
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class FailingTestData : IEnumerable<object[]>
        {
            private readonly IList<string> _testCaseFilenames = new List<string>
            {
                "Samples.Demo.UnitTests.Echoes.BillingFails.echo",
                "Samples.Demo.UnitTests.Echoes.ProvisioningFails.echo",
            };

            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var testCaseFilename in _testCaseFilenames)
                {
                    using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(testCaseFilename))
                    {
                        using (var streamReader = new StreamReader(resourceStream))
                        {
                            var echoes = new List<string>();
                            while (!streamReader.EndOfStream)
                            {
                                echoes.Add(streamReader.ReadLine());
                            }
                            yield return new object[] { new EchoReader(echoes) };
                        }
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
