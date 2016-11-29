﻿using Echo.UnitTests.Asserts;
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
        public void GetRecordingTarget_TTargetIsNotInterface_Throws()
        {
            // Arrange
            var recorder = new Recorder(invocationWritter: null);

            // Assert
            ExceptionAssert.Throws<NotSupportedException>(() =>
                    // Act
                    recorder.GetRecordingTarget<FakeDependency>(target: null));
        }

        [TestMethod]
        public void GetRecordingTarget_TTargetIsNotPublicInterface_Throws()
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
        public void GetRecordingTarget_DependencyThrows_InvocationWritterCalled()
        {
            // Arrange
            var writterMock = new Mock<IInvocationWritter>();
            var recorder = new Recorder(writterMock.Object);
            var dependencyMock = new Mock<IFakeDependency>();
            dependencyMock
                .Setup(x => x.GetRemoteResource())
                .Throws(new Exception());
            var dependencyUnderRecording = recorder.GetRecordingTarget<IFakeDependency>(dependencyMock.Object);

            // Act
            ExceptionAssert.Throws(() => dependencyUnderRecording.GetRemoteResource());

            // Assert
            writterMock.Verify(
                x => x.RecordInvocation(
                    It.Is<MethodInfo>(method => method.Name.Equals("GetRemoteResource")),
                    It.Is<object>(returnValue => returnValue == null),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }

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
                    It.Is<object>(returnValue => returnValue == null),
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
                    It.Is<object>(returnValue => returnValue == remoteResource),
                    It.Is<object[]>(arguments => arguments.Length == 0)),
                Times.Once);
        }
    }
}
