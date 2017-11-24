using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
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
        public void GetReplayingTarget_RecordingIsVoid_Pass()
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
                            typeof(IFakeTarget<object>)), 
                    });
            using (var recorder = new Player(readerMock.Object))
            {
                var targetUnderRecording = recorder.GetReplayingTarget<IFakeTarget<object>>();

                // Act
                targetUnderRecording.CallRemoteResource();

                // Assert
                readerMock.VerifyAll();
            }
        }

        [Fact]
        public void GetReplayingTarget_RecordingThrows_ReplayThrows()
        {
            // Arrange
            var expectedException = new FakeTargetException();
            var readerMock = new Mock<IInvocationReader>();
            readerMock
                .Setup(x => x.GetAllInvocations())
                .Returns(
                    new[]
                    {
                        new Invocation(
                            new object[0],
                            "CallRemoteResource",
                            new ExceptionInvocationResult(expectedException),
                            typeof(IFakeTarget<object>)),
                    });
            using (var recorder = new Player(readerMock.Object))
            {
                var targetUnderRecording = recorder.GetReplayingTarget<IFakeTarget<object>>();

                try
                {
                    // Act
                    targetUnderRecording.CallRemoteResource();
                }
                catch (FakeTargetException e)
                {
                    // Assert
                    Assert.Equal(expectedException, e);
                }
            }
        }

        [Fact]
        public void GetReplayingTarget_RecordingReturnsObject_ValueReturned()
        {
            // Arrange
            var expectedResource = new object();
            var readerMock = new Mock<IInvocationReader>();
            readerMock
                .Setup(x => x.GetAllInvocations())
                .Returns(
                    new[]
                    {
                        new Invocation(
                            new object[0],
                            "GetRemoteResource",
                            new ValueInvocationResult(expectedResource), 
                            typeof(IFakeTarget<object>)),
                    });
            using (var recorder = new Player(readerMock.Object))
            {
                var targetUnderRecording = recorder.GetReplayingTarget<IFakeTarget<object>>();

                // Act
                var actualResource = targetUnderRecording.GetRemoteResource();
                
                // Assert
                Assert.Equal(expectedResource, actualResource);
            }
        }

        [Fact]
        public void GetReplayingTarget_RecordingReturnsInt_ValueReturned()
        {
            // Arrange
            var expectedResource = 3;
            var readerMock = new Mock<IInvocationReader>();
            readerMock
                .Setup(x => x.GetAllInvocations())
                .Returns(
                    new[]
                    {
                        new Invocation(
                            new object[0],
                            "GetRemoteResource",
                            new ValueInvocationResult(3L),
                            typeof(IFakeTarget<int>)),
                    });
            using (var recorder = new Player(readerMock.Object))
            {
                var targetUnderRecording = recorder.GetReplayingTarget<IFakeTarget<int>>();

                // Act
                var actualResource = targetUnderRecording.GetRemoteResource();

                // Assert
                Assert.Equal(expectedResource, actualResource);
            }
        }

        [Fact]
        public void GetReplayingTarget_RecordingReturnsFloat_ValueReturned()
        {
            // Arrange
            var expectedResource = 0.3F;
            var readerMock = new Mock<IInvocationReader>();
            readerMock
                .Setup(x => x.GetAllInvocations())
                .Returns(
                    new[]
                    {
                        new Invocation(
                            new object[0],
                            "GetRemoteResource",
                            new ValueInvocationResult(0.3D),
                            typeof(IFakeTarget<float>)),
                    });
            using (var recorder = new Player(readerMock.Object))
            {
                var targetUnderRecording = recorder.GetReplayingTarget<IFakeTarget<float>>();

                // Act
                var actualResource = targetUnderRecording.GetRemoteResource();

                // Assert
                Assert.Equal(expectedResource, actualResource);
            }
        }

        [Fact]
        public async Task GetReplayingTarget_RecordingIsTask_Pass()
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
                            "CallRemoteResourceAsync",
                            InvocationResult.Void,
                            typeof(IFakeTargetAsync<object>)),
                    });
            using (var recorder = new Player(readerMock.Object))
            {
                var targetUnderRecording = recorder.GetReplayingTarget<IFakeTargetAsync<object>>();

                // Act
                await targetUnderRecording.CallRemoteResourceAsync();

                // Assert
                readerMock.VerifyAll();
            }
        }
    }
}
