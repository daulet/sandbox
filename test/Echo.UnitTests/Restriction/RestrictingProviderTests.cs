using Echo.Logging;
using Echo.Restriction;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Echo.UnitTests.Restriction
{
    public class RestrictingProviderTests
    {
        private readonly ILogger _logger = Mock.Of<ILogger>();

        [Fact]
        public void GetRestrictedTarget_NotPublicTarget_ThrowsNotSupportedException()
        {
            // Arrange
            var restrictingProvider = new RestrictingProvider(_logger);
            // Assert
            Assert.Throws<NotSupportedException>(() =>
                // Act
                restrictingProvider.GetRestrictedTarget<IFakeInternalTarget>(new FakeInternalTarget()));
        }

        [Fact]
        public void GetRestrictedTarget_NotInterfaceTarget_ThrowsNotSupportedException()
        {
            // Arrange
            var restrictingProvider = new RestrictingProvider(_logger);
            // Assert
            Assert.Throws<NotSupportedException>(() =>
                // Act
                restrictingProvider.GetRestrictedTarget(new FakeInternalTarget()));
        }

        [Fact]
        public void GetRestrictedTarget_UnrestrictedMethod_NotRestricted()
        {
            // Arrange
            var fakeTargetMock = new Mock<IFakeTarget>(MockBehavior.Strict);
            fakeTargetMock.Setup(x => x.UnrestrictedMethod());
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictedTarget(fakeTargetMock.Object);

            // Act
            restrictedTarget.UnrestrictedMethod();

            // Assert
            fakeTargetMock.Verify(x => x.UnrestrictedMethod(), Times.Once);
        }

        [Fact]
        public void GetRestrictedTarget_UnrestrictedMethodWithCustomAttribute_NotRestricted()
        {
            // Arrange
            var fakeTargetMock = new Mock<IFakeTarget>(MockBehavior.Strict);
            fakeTargetMock.Setup(x => x.UnrestrictedMethodWithCustomAttribute());
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictedTarget(fakeTargetMock.Object);

            // Act
            restrictedTarget.UnrestrictedMethodWithCustomAttribute();

            // Assert
            fakeTargetMock.Verify(x => x.UnrestrictedMethodWithCustomAttribute(), Times.Once);
        }

        [Fact]
        public void GetRestrictedTarget_RestrictedMethod_Restricted()
        {
            // Arrange
            var fakeTargetMock = new Mock<IFakeTarget>(MockBehavior.Strict);
            fakeTargetMock.Setup(x => x.RestrictedMethod());
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictedTarget(fakeTargetMock.Object);

            // Act
            restrictedTarget.RestrictedMethod();

            // Assert
            fakeTargetMock.Verify(x => x.RestrictedMethod(), Times.Never);
        }

        [Fact]
        public void GetRestrictedTarget_RestrictedMethodWithCustomAttribute_Restricted()
        {
            // Arrange
            var fakeTargetMock = new Mock<IFakeTarget>(MockBehavior.Strict);
            fakeTargetMock.Setup(x => x.RestrictedMethodWithCustomAttribute());
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictedTarget(fakeTargetMock.Object);

            // Act
            restrictedTarget.RestrictedMethodWithCustomAttribute();

            // Assert
            fakeTargetMock.Verify(x => x.RestrictedMethodWithCustomAttribute(), Times.Never);
        }

        [Fact]
        public void GetRestrictedTarget_RestrictedMethod_ObjectReturnValue_DefaultValueReturned()
        {
            // Arrange
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictedTarget(Mock.Of<IFakeTarget>());

            // Act
            var returnValue = restrictedTarget.RestrictedMethodWithObjectReturnValue<object>();

            // Assert
            Assert.Equal(null, returnValue);
        }

        [Fact]
        public void GetRestrictedTarget_RestrictedMethod_NullParameter_LogsInfoMessage()
        {
            // Arrange
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictedTarget(Mock.Of<IFakeTarget>());

            // Act
            restrictedTarget.RestrictedMethodWithParameter<object>(null);

            // Assert
            Mock.Get(_logger).Verify(x => x.Info(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetRestrictedTarget_RestrictedMethod_StructReturnValue_DefaultValueReturned()
        {
            // Arrange
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictedTarget(Mock.Of<IFakeTarget>());

            // Act
            var returnValue = restrictedTarget.RestrictedMethodWithStructReturnValue<Guid>();

            // Assert
            Assert.Equal(Guid.Empty, returnValue);
        }

        [Fact]
        public async Task GetRestrictedTarget_RestrictedMethod_TaskReturnValue_DefaultValueReturned()
        {
            // Arrange
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictedTarget(Mock.Of<IFakeTarget>());

            // Act
            await restrictedTarget.RestrictedMethodWithTaskReturnValue();
        }

        [Fact]
        public async Task GetRestrictedTarget_RestrictedMethod_TaskWithScalarValueReturnValue_DefaultValueReturned()
        {
            // Arrange
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictedTarget(Mock.Of<IFakeTarget>());

            // Act
            var returnValue = await restrictedTarget.RestrictedMethodWithTaskWithValueReturnValue<int>();

            // Assert
            Assert.Equal(0, returnValue);
        }

        [Fact]
        public async Task GetRestrictedTarget_RestrictedMethod_TaskWithObjectValueReturnValue_DefaultValueReturned()
        {
            // Arrange
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictedTarget(Mock.Of<IFakeTarget>());

            // Act
            var returnValue = await restrictedTarget.RestrictedMethodWithTaskWithValueReturnValue<string>();

            // Assert
            Assert.Equal(null, returnValue);
        }
    }
}
