using Echo.Core;
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
            var recorder = new Recorder(invocationWriter: null);

            // Assert
            ExceptionAssert.Throws<NotSupportedException>(() =>
                // Act
                recorder.GetRecordingTarget<FakeTarget>(target: null));
        }

        [TestMethod]
        public void GetRecordingTarget_TTargetIsNotPublicInterface_ThrowsNotSupportedException()
        {
            // Arrange
            var recorder = new Recorder(invocationWriter: null);
            Assert.IsFalse(typeof(IInternalFakeTarget).IsPublic, "Interface can't be public for test to be valid");

            // Assert
            ExceptionAssert.Throws<NotSupportedException>(() =>
                // Act
                recorder.GetRecordingTarget<IInternalFakeTarget>(target: null));
        }

        [TestMethod]
        public void GetRecordingTarget_ReturnTypeIsVoid_InvocationWriterCalled()
        {
            // Arrange
            var writerMock = new Mock<IInvocationListener>();
            var recorder = new Recorder(writerMock.Object);
            var targetMock = new Mock<IFakeTarget>();
            var targetUnderRecording = recorder.GetRecordingTarget<IFakeTarget>(targetMock.Object);

            // Act
            targetUnderRecording.CallRemoteResource();

            // Assert
            writerMock.Verify(
                x => x.WriteInvocation<IFakeTarget>(
                    It.Is<MethodInfo>(method => method.Name.Equals("CallRemoteResource")),
                    It.Is<InvocationResult>(returnValue => returnValue == InvocationResult.Void),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        [TestMethod]
        public void GetRecordingTarget_TargetThrows_InvocationWriterCalled()
        {
            // Arrange
            var writerMock = new Mock<IInvocationListener>();
            var recorder = new Recorder(writerMock.Object);
            var targetMock = new Mock<IFakeTarget>();
            var expectedException = new FakeTargetException();
            targetMock
                .Setup(x => x.GetRemoteResource())
                .Throws(expectedException);
            var targetUnderRecording = recorder.GetRecordingTarget<IFakeTarget>(targetMock.Object);

            // Act
            ExceptionAssert.Throws<FakeTargetException>(() => targetUnderRecording.GetRemoteResource());

            // Assert
            writerMock.Verify(
                x => x.WriteInvocation<IFakeTarget>(
                    It.Is<MethodInfo>(method => method.Name.Equals("GetRemoteResource")),
                    It.Is<InvocationResult>(returnValue
                        => returnValue is ExceptionInvocationResult
                        && (returnValue as ExceptionInvocationResult).ThrownException == expectedException),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        [TestMethod]
        public void GetRecordingTarget_ReturnsValue_InvocationWriterCalled()
        {
            // Arrange
            var writerMock = new Mock<IInvocationListener>();
            var recorder = new Recorder(writerMock.Object);
            var targetMock = new Mock<IFakeTarget>();
            var expectedResource = new object();
            targetMock
                .Setup(x => x.GetRemoteResource())
                .Returns(expectedResource);
            var targetUnderRecording = recorder.GetRecordingTarget<IFakeTarget>(targetMock.Object);

            // Act
            var actualResource = targetUnderRecording.GetRemoteResource();

            // Assert
            Assert.AreEqual(expectedResource, actualResource);
            writerMock.Verify(
                x => x.WriteInvocation<IFakeTarget>(
                    It.Is<MethodInfo>(method => method.Name.Equals("GetRemoteResource")),
                    It.Is<InvocationResult>(returnValue
                        => returnValue is ValueInvocationResult
                        && (returnValue as ValueInvocationResult).ReturnedValue == expectedResource),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        [TestMethod]
        public async Task GetRecordingTarget_ReturnValueIsTask_InvocationWriterCalled()
        {
            // Arrange
            var writerMock = new Mock<IInvocationListener>();
            var recorder = new Recorder(writerMock.Object);
            var targetMock = new Mock<IFakeTargetAsync>();
            targetMock
                .Setup(x => x.CallRemoteResourceAsync())
                .Returns(Task.CompletedTask);
            var targetUnderRecording = recorder.GetRecordingTarget<IFakeTargetAsync>(targetMock.Object);

            // Act
            await targetUnderRecording.CallRemoteResourceAsync();

            // Assert
            writerMock.Verify(
                x => x.WriteInvocation<IFakeTargetAsync>(
                    It.Is<MethodInfo>(method => method.Name.Equals("CallRemoteResourceAsync")),
                    It.Is<InvocationResult>(returnValue => returnValue == InvocationResult.Void),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        [TestMethod]
        public async Task GetRecordingTarget_ReturnValueIsTask_TargetThrows_InvocationWriterCalled()
        {
            // Arrange
            var writerMock = new Mock<IInvocationListener>();
            var recorder = new Recorder(writerMock.Object);
            var targetMock = new Mock<IFakeTargetAsync>();
            var expectedException = new FakeTargetException();
            targetMock
                .Setup(x => x.CallRemoteResourceAsync())
                .Returns(Task.FromException(expectedException));
            var targetUnderRecording = recorder.GetRecordingTarget<IFakeTargetAsync>(targetMock.Object);

            // Act
            await ExceptionAssert.ThrowsAsync<FakeTargetException>(
                async () => await targetUnderRecording.CallRemoteResourceAsync());

            // Assert
            writerMock.Verify(
                x => x.WriteInvocation<IFakeTargetAsync>(
                    It.Is<MethodInfo>(method => method.Name.Equals("CallRemoteResourceAsync")),
                    It.Is<InvocationResult>(returnValue
                        => returnValue is ExceptionInvocationResult
                        && (returnValue as ExceptionInvocationResult).ThrownException == expectedException),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        [TestMethod]
        public async Task GetRecordingTarget_ReturnValueIsTaskWithResult_InvocationWriterCalled()
        {
            // Arrange
            var writerMock = new Mock<IInvocationListener>();
            var recorder = new Recorder(writerMock.Object);
            var targetMock = new Mock<IFakeTargetAsync>();
            var expectedResource = new object();
            targetMock
                .Setup(x => x.GetRemoteResourceAsync())
                .ReturnsAsync(expectedResource);
            var targetUnderRecording = recorder.GetRecordingTarget<IFakeTargetAsync>(targetMock.Object);

            // Act
            var actualResource = await targetUnderRecording.GetRemoteResourceAsync();

            // Assert
            Assert.AreEqual(expectedResource, actualResource);
            writerMock.Verify(
                x => x.WriteInvocation<IFakeTargetAsync>(
                    It.Is<MethodInfo>(method => method.Name.Equals("GetRemoteResourceAsync")),
                    It.Is<InvocationResult>(returnValue
                        => returnValue is ValueInvocationResult
                        && (returnValue as ValueInvocationResult).ReturnedValue == expectedResource),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

        [TestMethod]
        public async Task GetRecordingTarget_ReturnValueIsTaskWithResult_TargetThrows_InvocationWriterCalled()
        {
            // Arrange
            var writerMock = new Mock<IInvocationListener>();
            var recorder = new Recorder(writerMock.Object);
            var targetMock = new Mock<IFakeTargetAsync>();
            var expectedException = new FakeTargetException();
            targetMock
                .Setup(x => x.GetRemoteResourceAsync())
                .Returns(Task.FromException<object>(expectedException));
            var targetUnderRecording = recorder.GetRecordingTarget<IFakeTargetAsync>(targetMock.Object);

            // Act
            await ExceptionAssert.ThrowsAsync<FakeTargetException>(
                async () => await targetUnderRecording.GetRemoteResourceAsync());

            // Assert
            writerMock.Verify(
                x => x.WriteInvocation<IFakeTargetAsync>(
                    It.Is<MethodInfo>(method => method.Name.Equals("GetRemoteResourceAsync")),
                    It.Is<InvocationResult>(returnValue
                        => returnValue is ExceptionInvocationResult
                        && (returnValue as ExceptionInvocationResult).ThrownException == expectedException),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }
    }
}
