using Echo.Logging;
using Echo.Restriction;
using Moq;
using Xunit;

namespace Echo.UnitTests.Restriction
{
    public class RestrictingProviderTests
    {
        private readonly ILogger _logger = Mock.Of<ILogger>();

        [Fact]
        public void GetRestrictingTarget_UnrestrictedMethod_NoRestriction()
        {
            // Arrange
            var fakeTargetMock = new Mock<IFakeTarget>(MockBehavior.Strict);
            fakeTargetMock.Setup(x => x.UnrestrictedMethod());
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictingTarget(fakeTargetMock.Object);

            // Act
            restrictedTarget.UnrestrictedMethod();

            // Assert
            fakeTargetMock.Verify(x => x.UnrestrictedMethod(), Times.Once);
        }

        [Fact]
        public void GetRestrictingTarget_UnrestrictedMethodWithCustomAttribute_NoRestriction()
        {
            // Arrange
            var fakeTargetMock = new Mock<IFakeTarget>(MockBehavior.Strict);
            fakeTargetMock.Setup(x => x.UnrestrictedMethodWithCustomAttribute());
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictingTarget(fakeTargetMock.Object);

            // Act
            restrictedTarget.UnrestrictedMethodWithCustomAttribute();

            // Assert
            fakeTargetMock.Verify(x => x.UnrestrictedMethodWithCustomAttribute(), Times.Once);
        }

        [Fact]
        public void GetRestrictingTarget_RestrictedMethod_NoRestriction()
        {
            // Arrange
            var fakeTargetMock = new Mock<IFakeTarget>(MockBehavior.Strict);
            fakeTargetMock.Setup(x => x.RestrictedMethod());
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictingTarget(fakeTargetMock.Object);

            // Act
            restrictedTarget.RestrictedMethod();

            // Assert
            fakeTargetMock.Verify(x => x.RestrictedMethod(), Times.Never);
        }

        [Fact]
        public void GetRestrictingTarget_RestrictedMethodWithCustomAttribute_NoRestriction()
        {
            // Arrange
            var fakeTargetMock = new Mock<IFakeTarget>(MockBehavior.Strict);
            fakeTargetMock.Setup(x => x.RestrictedMethodWithCustomAttribute());
            var restrictingProvider = new RestrictingProvider(_logger);
            var restrictedTarget = restrictingProvider.GetRestrictingTarget(fakeTargetMock.Object);

            // Act
            restrictedTarget.RestrictedMethodWithCustomAttribute();

            // Assert
            fakeTargetMock.Verify(x => x.RestrictedMethodWithCustomAttribute(), Times.Never);
        }
    }
}
