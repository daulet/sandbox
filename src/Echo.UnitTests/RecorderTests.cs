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
                    recorder.GetRecordingTarget<FakeDependency>(target: null));
        }

        [TestMethod]
        public void GetRecordingTarget_TTargetIsNotPublicInterface_ThrowsNotSupportedException()
        {
            // Arrange
            var recorder = new Recorder(invocationWritter: null);
            Assert.IsFalse(typeof(IInternalFakeDependency).IsPublic, "Interface can't be public for test to be valid");

            // Assert
            ExceptionAssert.Throws<NotSupportedException>(() =>
                    // Act
                    recorder.GetRecordingTarget<IInternalFakeDependency>(target: null));
        }

        [TestMethod]
        public void GetRecordingTarget_ReturnTypeIsVoid_InvocationWritterCalled()
        {
            // Arrange
            var writterMock = new Mock<IInvocationWritter>();
            var recorder = new Recorder(writterMock.Object);
            var dependencyMock = new Mock<IFakeDependency>();
            dependencyMock
                .Setup(x => x.GetRemoteResource());
            var dependencyUnderRecording = recorder.GetRecordingTarget<IFakeDependency>(dependencyMock.Object);

            // Act
            dependencyUnderRecording.CallRemoteResource();

            // Assert
            writterMock.Verify(
                x => x.RecordInvocation(
                    It.Is<MethodInfo>(method => method.Name.Equals("CallRemoteResource")),
                    It.Is<InvocationResult>(returnValue => returnValue == InvocationResult.Void),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        [TestMethod]
        public void GetRecordingTarget_DependencyThrows_InvocationWritterCalled()
        {
            // Arrange
            var writterMock = new Mock<IInvocationWritter>();
            var recorder = new Recorder(writterMock.Object);
            var dependencyMock = new Mock<IFakeDependency>();
            var thrownException = new FakeDependencyException();
            dependencyMock
                .Setup(x => x.GetRemoteResource())
                .Throws(thrownException);
            var dependencyUnderRecording = recorder.GetRecordingTarget<IFakeDependency>(dependencyMock.Object);

            // Act
            ExceptionAssert.Throws<FakeDependencyException>(() => dependencyUnderRecording.GetRemoteResource());

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

        // TODO add test for dependency that did return value

        [TestMethod]
        public async Task GetRecordingTarget_ReturnValueIsTask_InvocationWritterCalled()
        {
            // Arrange
            var writterMock = new Mock<IInvocationWritter>();
            var recorder = new Recorder(writterMock.Object);
            var dependencyMock = new Mock<IFakeDependencyAsync>();
            dependencyMock
                .Setup(x => x.CallRemoteResourceAsync())
                .Returns(Task.CompletedTask);
            var dependencyUnderRecording = recorder.GetRecordingTarget<IFakeDependencyAsync>(dependencyMock.Object);

            // Act
            await dependencyUnderRecording.CallRemoteResourceAsync();

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
            var dependencyMock = new Mock<IFakeDependencyAsync>();
            var thrownException = new FakeDependencyException();
            dependencyMock
                .Setup(x => x.CallRemoteResourceAsync())
                .Throws(thrownException);
            var dependencyUnderRecording = recorder.GetRecordingTarget<IFakeDependencyAsync>(dependencyMock.Object);

            // Act
            await ExceptionAssert.ThrowsAsync<FakeDependencyException>(
                async () => await dependencyUnderRecording.CallRemoteResourceAsync());

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
            var dependencyMock = new Mock<IFakeDependencyAsync>();
            var remoteResource = new object();
            dependencyMock
                .Setup(x => x.GetRemoteResourceAsync())
                .ReturnsAsync(remoteResource);
            var dependencyUnderRecording = recorder.GetRecordingTarget<IFakeDependencyAsync>(dependencyMock.Object);

            // Act
            await dependencyUnderRecording.GetRemoteResourceAsync();

            // Assert
            writterMock.Verify(
                x => x.RecordInvocation(
                    It.Is<MethodInfo>(method => method.Name.Equals("GetRemoteResourceAsync")),
                    It.Is<InvocationResult>(returnValue
                        => returnValue is ValueInvocationResult
                        && (returnValue as ValueInvocationResult).ReturnValue == remoteResource),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        [TestMethod]
        public async Task GetRecordingTarget_ReturnValueIsTaskWithResult_TargetThrows_InvocationWritterCalled()
        {
            // Arrange
            var writterMock = new Mock<IInvocationWritter>();
            var recorder = new Recorder(writterMock.Object);
            var dependencyMock = new Mock<IFakeDependencyAsync>();
            var thrownException = new FakeDependencyException();
            dependencyMock
                .Setup(x => x.GetRemoteResourceAsync())
                .Throws(thrownException);
            var dependencyUnderRecording = recorder.GetRecordingTarget<IFakeDependencyAsync>(dependencyMock.Object);

            // Act
            await ExceptionAssert.ThrowsAsync<FakeDependencyException>(
                async () => await dependencyUnderRecording.GetRemoteResourceAsync());

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
