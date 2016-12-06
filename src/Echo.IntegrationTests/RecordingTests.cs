using Echo.IntegrationTests.Source;
using Moq;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Echo.IntegrationTests
{
    public class RecordingTests
    {
        //[Fact]
        // TODO disabled for now - will fail deterministically due to differences in timestamp
        // TODO rename serviceProviderMock to providerMock - same applies to Demo project
        public void Purchase_RecordOutput1_MatchesEmbeddedResource()
        {
            var echoFileName = "Echo.IntegrationTests.Echoes.output1.echo";

            // Arrange

            var billingMock = new Mock<IBilling>();
            billingMock
                .Setup(x => x.GetQuote(It.Is<QuoteRequest>(
                        quoteRequest => quoteRequest.Service == ServiceType.Entertainment)
                ))
                .Returns(new QuoteResponse()
                {
                    Price = 10.50,
                });

            billingMock
                .Setup(x => x.Charge(It.IsAny<PaymentRequest>()))
                .Returns(new PaymentResponse()
                {
                    Result = PaymentCode.Success,
                });

            var serviceProviderMock = new Mock<IProvider>();
            serviceProviderMock
                .Setup(x => x.Provision(It.IsAny<ProvisioningRequest>()))
                .Returns(new ProvisioningResponse()
                {
                    ProvisionedServices = new[] { ServiceType.Entertainment, ServiceType.Laundry, },
                });

            // write all echoes to a file
            using (var output = new StreamWriter(echoFileName))
            {
                // setup recording

                var writer = new EchoWriter(output);
                var recorder = new Recorder(writer);

                var recordedBilling = recorder.GetRecordingTarget<IBilling>(billingMock.Object);
                var recordedServiceProvider = recorder.GetRecordingTarget<IProvider>(serviceProviderMock.Object);
                var actualEndpoint = new Endpoint(recordedBilling, recordedServiceProvider);
                var recordedEndpoint = recorder.GetRecordingTarget<IEndpoint>(actualEndpoint);

                // Act

                recordedEndpoint.Purchase(new PurchaseRequest()
                {
                    Customer = new User()
                    {
                        FullName = "John Smith",
                        Identifier = Guid.NewGuid(),
                    },
                    Payment = new CreditCardPaymentInstrument()
                    {
                        CardExpirationDate = DateTime.UtcNow.AddYears(1),
                        CardNumber = long.MaxValue,
                        CardOwner = "John Smith SR",
                        CardProvider = CreditCardProvider.Visa,
                    },
                    ServiceType = ServiceType.Entertainment,
                });
            }

            // Assert

            string expectedEcho;
            using (var expectedStreamReader = new StreamReader(echoFileName))
            {
                expectedEcho = expectedStreamReader.ReadToEnd();
            }

            string actualEcho;
            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(echoFileName))
            {
                using (var actualStreamReader = new StreamReader(resourceStream))
                {
                    actualEcho = actualStreamReader.ReadToEnd();
                }
            }

            // TODO compare all but timestamps
            Assert.Equal(expectedEcho, actualEcho);
        }
    }
}
