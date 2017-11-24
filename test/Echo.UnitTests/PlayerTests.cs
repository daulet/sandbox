using System;
using System.IO;
using System.Reflection;
using Echo.Core;
using Echo.UnitTests.Fakes;
using Moq;
using Xunit;

namespace Echo.UnitTests
{
    public class PlayerTests
    {
        [Fact]
        public void GetReplayingTarget_TTargetIsNotInterface_ThrowsNotSupportedException()
        {
            // Arrange
            using (var recorder = new Player(invocationReader: null))
            {
                // Assert
                Assert.Throws<NotSupportedException>(() =>
                    // Act
                    recorder.GetReplayingTarget<FakeTarget>());
            }
        }

        [Fact]
        public void GetReplayingTarget_TTargetIsNotPublicInterface_ThrowsNotSupportedException()
        {
            // Arrange
            using (var recorder = new Player(invocationReader: null))
            {
                Assert.False(typeof(IInternalFakeTarget).GetTypeInfo().IsPublic, "Interface can't be public for test to be valid");

                // Assert
                Assert.Throws<NotSupportedException>(() =>
                    // Act
                    recorder.GetReplayingTarget<IInternalFakeTarget>());
            }
        }

        [Fact]
        public void GetReplayingTarget_ReturnTypeIsVoid_InvocationWriterCalled()
        {
            // Arrange
            var readerMock = new Mock<IInvocationReader>();
            readerMock
                .Setup(x => x.GetAllInvocations())
                .Returns(
                    new[]
                    {
                        new Invocation(
                            new object[0],
                            "CallRemoteResource",
                            InvocationResult.Void,
                            typeof(IFakeTarget)), 
                    });
            using (var recorder = new Player(readerMock.Object))
            {
                var targetUnderRecording = recorder.GetReplayingTarget<IFakeTarget>();

                // Act
                targetUnderRecording.CallRemoteResource();

                // Assert
                readerMock.VerifyAll();
            }
        }
    }
}
