using Echo;
using Moq;
using Samples.Demo.Source;
using System;
using System.IO;

namespace Samples.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Recording");
            
            using (var output = new StreamWriter("ProvisioningFails.echo"))
            {
                Record_ProvisioningFails_RefundIsMade(output);
            }

            Console.ReadKey();
        }

        private static void Record_ProvisioningFails_RefundIsMade(StreamWriter streamWriter)
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

            var providerMock = new Mock<IProvider>();
            providerMock
                .Setup(x => x.Provision(It.IsAny<ProvisioningRequest>()))
                .Throws(new ProvisioningFailureException());

            var purchaseRequest = new PurchaseRequest()
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
            };

            // Act

            try
            {
                Record(streamWriter, billingMock.Object, providerMock.Object, purchaseRequest);
            }
            catch (PurchaseFailureException)
            {
                // the Purchase is expected to fail since IProvider failed
            }
        }

        private static void Record(StreamWriter streamWriter, IBilling billing, IProvider provider, PurchaseRequest purchaseRequest)
        {
            // setup recording

            var writer = new EchoWriter(streamWriter);
            var recorder = new Recorder(writer);

            var recordedBilling = recorder.GetRecordingTarget<IBilling>(billing);
            var recordedProvider = recorder.GetRecordingTarget<IProvider>(provider);
            var actualEndpoint = new Endpoint(recordedBilling, recordedProvider);
            var recordedEndpoint = recorder.GetRecordingTarget<IEndpoint>(actualEndpoint);

            // Act

            recordedEndpoint.Purchase(purchaseRequest);
        }
    }
}
