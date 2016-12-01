using Echo;
using Echo.UnitTesting;
using Moq;
using Samples.MultiDependency.Target;
using System;
using System.IO;

namespace Samples.MultiDependency
{
    internal class Program
    {
        private const string EchoFileName = "output.echo";

        private static void Main(string[] args)
        {
            //Record();
            Test();
            // TODO add intentionally broken implementations, and replay echoes on them

            Console.ReadKey();
        }

        private static void Record()
        {
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
            using (var output = new StreamWriter(EchoFileName))
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
        }

        private static void Test()
        {
            using (var streamReader = new StreamReader(EchoFileName))
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
}
