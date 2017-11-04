using System;
using System.IO;
using System.Reflection;
using Echo.IntegrationTests.Subject;
using Moq;
using Xunit;

namespace Echo.IntegrationTests.Recording
{
    public class RecorderTests
    {
        [Fact]
        public void Purchase_RecordHappyCase_MatchesEmbeddedResource()
        {
            const string echoFileName = "Echo.IntegrationTests.Recording.HappyCase.echo";

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
                .Returns(new ProvisioningResponse()
                {
                    ProvisionedServices = new[] {ServiceType.Entertainment, ServiceType.Laundry,},
                });

            // write all echoes to a file
            using (var streamWriter = new StreamWriter(echoFileName))
            {
                // setup recording
                var recorder = new Recorder(streamWriter);

                var recordedBilling = recorder.GetRecordingTarget<IBilling>(billingMock.Object);
                var recordedProvider = recorder.GetRecordingTarget<IProvider>(providerMock.Object);
                var actualEndpoint = new Endpoint(recordedBilling, recordedProvider);
                var recordedEndpoint = recorder.GetRecordingTarget<IEndpoint>(actualEndpoint);

                // Act

                recordedEndpoint.Purchase(new PurchaseRequest()
                {
                    Customer = new User()
                    {
                        FullName = "John Smith",
                        Identifier = new Guid("43fe7aaa-7be9-46e0-87e7-63c9a6f3a2ad"),
                    },
                    Payment = new CreditCardPaymentInstrument()
                    {
                        CardExpirationDate = DateTime.MaxValue,
                        CardNumber = long.MaxValue,
                        CardOwner = "John Smith SR",
                        CardProvider = CreditCardProvider.Visa,
                    },
                    ServiceType = ServiceType.Entertainment,
                });
            }

            // Assert

            Assert.Equal(ReadFile(echoFileName), ReadResource(echoFileName));
        }

        [Fact]
        public void Purchase_RecordBillingFails_MatchesEmbeddedResource()
        {
            const string echoFileName = "Echo.IntegrationTests.Recording.BillingFails.echo";

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
                    Result = PaymentCode.Declined,
                });

            var providerMock = new Mock<IProvider>(MockBehavior.Strict);

            // write all echoes to a file
            using (var streamWriter = new StreamWriter(echoFileName))
            {
                // setup recording
                var recorder = new Recorder(streamWriter);

                var recordedBilling = recorder.GetRecordingTarget<IBilling>(billingMock.Object);
                var recordedProvider = recorder.GetRecordingTarget<IProvider>(providerMock.Object);
                var actualEndpoint = new Endpoint(recordedBilling, recordedProvider);
                var recordedEndpoint = recorder.GetRecordingTarget<IEndpoint>(actualEndpoint);

                // Act

                try
                {
                    recordedEndpoint.Purchase(new PurchaseRequest()
                    {
                        Customer = new User()
                        {
                            FullName = "John Smith",
                            Identifier = new Guid("43fe7aaa-7be9-46e0-87e7-63c9a6f3a2ad"),
                        },
                        Payment = new CreditCardPaymentInstrument()
                        {
                            CardExpirationDate = DateTime.MaxValue,
                            CardNumber = long.MaxValue,
                            CardOwner = "John Smith SR",
                            CardProvider = CreditCardProvider.Visa,
                        },
                        ServiceType = ServiceType.Entertainment,
                    });
                }
                catch (PurchaseFailureException)
                {
                    // the Purchase is expected to fail since IBilling failed
                }
            }

            // Assert

            Assert.Equal(ReadFile(echoFileName), ReadResource(echoFileName));
        }

        [Fact]
        public void Purchase_RecordProvisioningFails_MatchesEmbeddedResource()
        {
            const string echoFileName = "Echo.IntegrationTests.Recording.ProvisioningFails.echo";

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

            // write all echoes to a file
            using (var streamWriter = new StreamWriter(echoFileName))
            {
                // setup recording
                var recorder = new Recorder(streamWriter);

                var recordedBilling = recorder.GetRecordingTarget<IBilling>(billingMock.Object);
                var recordedProvider = recorder.GetRecordingTarget<IProvider>(providerMock.Object);
                var actualEndpoint = new Endpoint(recordedBilling, recordedProvider);
                var recordedEndpoint = recorder.GetRecordingTarget<IEndpoint>(actualEndpoint);

                // Act

                try
                {
                    recordedEndpoint.Purchase(new PurchaseRequest()
                    {
                        Customer = new User()
                        {
                            FullName = "John Smith",
                            Identifier = new Guid("43fe7aaa-7be9-46e0-87e7-63c9a6f3a2ad"),
                        },
                        Payment = new CreditCardPaymentInstrument()
                        {
                            CardExpirationDate = DateTime.MaxValue,
                            CardNumber = long.MaxValue,
                            CardOwner = "John Smith SR",
                            CardProvider = CreditCardProvider.Visa,
                        },
                        ServiceType = ServiceType.Entertainment,
                    });
                }
                catch (PurchaseFailureException)
                {
                    // the Purchase is expected to fail since IProvider failed
                }
            }

            // Assert

            Assert.Equal(ReadFile(echoFileName), ReadResource(echoFileName));
        }

        private static string ReadFile(string filename)
        {
            using (var expectedStreamReader = new StreamReader(filename))
            {
                return expectedStreamReader.ReadToEnd()
                    .Replace(Environment.NewLine, "\n");
            }
        }

        private static string ReadResource(string resourceName)
        {
            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var actualStreamReader = new StreamReader(resourceStream))
                {
                    return actualStreamReader.ReadToEnd()
                        .Replace(Environment.NewLine, "\n");
                }
            }
        }
    }
}
