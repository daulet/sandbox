using Echo.UnitTests.Asserts;
using Echo.UnitTests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Echo.UnitTests
{
    [TestClass]
    public class RecorderTests
    {
        [TestMethod]
        public void GetRecordingTarget_TTargetIsNotInterface_ThrowsNotSupportedException()
        {
            // Arrange
            var recorder = new Recorder(invocationWritter: null);

            // Assert
            ExceptionAssert.Throws<NotSupportedException>(() =>
                    // Act
                    recorder.GetRecordingTarget<FakeTarget>(target: null));
        }

        [TestMethod]
        public void GetRecordingTarget_TTargetIsNotPublicInterface_ThrowsNotSupportedException()
        {
            // Arrange
            var recorder = new Recorder(invocationWritter: null);
            Assert.IsFalse(typeof(IInternalFakeTarget).IsPublic, "Interface can't be public for test to be valid");

            // Assert
            ExceptionAssert.Throws<NotSupportedException>(() =>
                    // Act
                    recorder.GetRecordingTarget<IInternalFakeTarget>(target: null));
        }

        [TestMethod]
        public void GetRecordingTarget_ReturnTypeIsVoid_InvocationWritterCalled()
        {
            // Arrange
            var writterMock = new Mock<IInvocationWritter>();
            var recorder = new Recorder(writterMock.Object);
            var targetMock = new Mock<IFakeTarget>();
            targetMock
                .Setup(x => x.GetRemoteResource());
            var targetUnderRecording = recorder.GetRecordingTarget<IFakeTarget>(targetMock.Object);

            // Act
            targetUnderRecording.CallRemoteResource();

            // Assert
            writterMock.Verify(
                x => x.RecordInvocation(
                    It.Is<MethodInfo>(method => method.Name.Equals("CallRemoteResource")),
                    It.Is<InvocationResult>(returnValue => returnValue == InvocationResult.Void),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        [TestMethod]
        public void GetRecordingTarget_targetThrows_InvocationWritterCalled()
        {
            // Arrange
            var writterMock = new Mock<IInvocationWritter>();
            var recorder = new Recorder(writterMock.Object);
            var targetMock = new Mock<IFakeTarget>();
            var thrownException = new FakeTargetException();
            targetMock
                .Setup(x => x.GetRemoteResource())
                .Throws(thrownException);
            var targetUnderRecording = recorder.GetRecordingTarget<IFakeTarget>(targetMock.Object);

            // Act
            ExceptionAssert.Throws<FakeTargetException>(() => targetUnderRecording.GetRemoteResource());

            // Assert
            writterMock.Verify(
                x => x.RecordInvocation(
                    It.Is<MethodInfo>(method => method.Name.Equals("GetRemoteResource")),
                    It.Is<InvocationResult>(returnValue
                        => returnValue is ExceptionInvocationResult
                        && (returnValue as ExceptionInvocationResult).ThrownException == thrownException),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        // TODO add test for target that did return value

        [TestMethod]
        public async Task GetRecordingTarget_ReturnValueIsTask_InvocationWritterCalled()
        {
            // Arrange
            var writterMock = new Mock<IInvocationWritter>();
            var recorder = new Recorder(writterMock.Object);
            var targetMock = new Mock<IFakeTargetAsync>();
            targetMock
                .Setup(x => x.CallRemoteResourceAsync())
                .Returns(Task.CompletedTask);
            var targetUnderRecording = recorder.GetRecordingTarget<IFakeTargetAsync>(targetMock.Object);

            // Act
            await targetUnderRecording.CallRemoteResourceAsync();

            // Assert
            writterMock.Verify(
                x => x.RecordInvocation(
                    It.Is<MethodInfo>(method => method.Name.Equals("CallRemoteResourceAsync")),
                    It.Is<InvocationResult>(returnValue => returnValue == InvocationResult.Void),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        [TestMethod]
        public async Task GetRecordingTarget_ReturnValueIsTask_TargetThrows_InvocationWritterCalled()
        {
            // Arrange
            var writterMock = new Mock<IInvocationWritter>();
            var recorder = new Recorder(writterMock.Object);
            var targetMock = new Mock<IFakeTargetAsync>();
            var thrownException = new FakeTargetException();
            targetMock
                .Setup(x => x.CallRemoteResourceAsync())
                .Returns(Task.FromException(thrownException));
            var targetUnderRecording = recorder.GetRecordingTarget<IFakeTargetAsync>(targetMock.Object);

            // Act
            await ExceptionAssert.ThrowsAsync<FakeTargetException>(
                async () => await targetUnderRecording.CallRemoteResourceAsync());

            // Assert
            writterMock.Verify(
                x => x.RecordInvocation(
                    It.Is<MethodInfo>(method => method.Name.Equals("CallRemoteResourceAsync")),
                    It.Is<InvocationResult>(returnValue
                        => returnValue is ExceptionInvocationResult
                        && (returnValue as ExceptionInvocationResult).ThrownException == thrownException),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        [TestMethod]
        public async Task GetRecordingTarget_ReturnValueIsTaskWithResult_InvocationWritterCalled()
        {
            // Arrange
            var writterMock = new Mock<IInvocationWritter>();
            var recorder = new Recorder(writterMock.Object);
            var targetMock = new Mock<IFakeTargetAsync>();
            var remoteResource = new object();
            targetMock
                .Setup(x => x.GetRemoteResourceAsync())
                .ReturnsAsync(remoteResource);
            var targetUnderRecording = recorder.GetRecordingTarget<IFakeTargetAsync>(targetMock.Object);

            // Act
            await targetUnderRecording.GetRemoteResourceAsync();

            // Assert
            writterMock.Verify(
                x => x.RecordInvocation(
                    It.Is<MethodInfo>(method => method.Name.Equals("GetRemoteResourceAsync")),
                    It.Is<InvocationResult>(returnValue
                        => returnValue is ValueInvocationResult
                        && (returnValue as ValueInvocationResult).ReturnedValue == remoteResource),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        [TestMethod]
        public async Task GetRecordingTarget_ReturnValueIsTaskWithResult_TargetThrows_InvocationWritterCalled()
        {
            // Arrange
            var writterMock = new Mock<IInvocationWritter>();
            var recorder = new Recorder(writterMock.Object);
            var targetMock = new Mock<IFakeTargetAsync>();
            var thrownException = new FakeTargetException();
            targetMock
                .Setup(x => x.GetRemoteResourceAsync())
                .Returns(Task.FromException<object>(thrownException));
            var targetUnderRecording = recorder.GetRecordingTarget<IFakeTargetAsync>(targetMock.Object);

            // Act
            await ExceptionAssert.ThrowsAsync<FakeTargetException>(
                async () => await targetUnderRecording.GetRemoteResourceAsync());

            // Assert
            writterMock.Verify(
                x => x.RecordInvocation(
                    It.Is<MethodInfo>(method => method.Name.Equals("GetRemoteResourceAsync")),
                    It.Is<InvocationResult>(returnValue
                        => returnValue is ExceptionInvocationResult
                        && (returnValue as ExceptionInvocationResult).ThrownException == thrownException),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        // TODO add asserts to Act return values - currently only spying on callbacks
    }
}
