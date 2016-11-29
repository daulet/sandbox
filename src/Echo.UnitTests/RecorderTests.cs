using Echo.UnitTests.Asserts;
using Echo.UnitTests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reflection;

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
            var dependency = new FakeDependency();

            // Assert
            ExceptionAssert.Throws<NotSupportedException>(() =>
                // Act
                recorder.GetRecordingTarget<FakeDependency>(dependency));
        }

        [TestMethod]
        public void GetRecordingTarget_DependencyThrows_InvocationWritterCalled()
        {
            // Arrange
            var writterMock = new Mock<IInvocationWritter>(MockBehavior.Strict);
            var recorder = new Recorder(writterMock.Object);
            var dependencyMock = new Mock<IFakeDependency>(MockBehavior.Strict);
            dependencyMock
                .Setup(x => x.GetRemoteResource())
                .Throws(new Exception());
            var dependencyUnderRecording = recorder.GetRecordingTarget<IFakeDependency>(dependencyMock.Object);

            // Act
            ExceptionAssert.Throws(() => dependencyUnderRecording.GetRemoteResource());

            // Assert
            writterMock.Verify(x =>
                x.RecordInvocation(
                    It.Is<MethodInfo>(method => method.Name.Equals("GetRemoteResource")),
                    It.Is<object>(returnValue => returnValue == null),
                    It.Is<object[]>(arguments => arguments.Length == 0)));
        }
    }
}
