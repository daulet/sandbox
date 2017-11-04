﻿using Moq;
using System;
using System.IO;
using System.Reflection;
using Echo.IntegrationTests.Subject;
using Xunit;

namespace Echo.IntegrationTests
{
    public class RecordingTests
    {
        [Fact]
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

            var providerMock = new Mock<IProvider>();
            providerMock
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

            Assert.Equal(expectedEcho, actualEcho);
        }
    }
}
