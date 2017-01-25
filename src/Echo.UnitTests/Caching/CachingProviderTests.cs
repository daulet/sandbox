using System;
using Echo.Caching;
using Moq;
using Xunit;

namespace Echo.UnitTests.Caching
{
    public class CachingProviderTests
    {
        [Fact]
        public void GetCachingTarget_NotCachedMethod_NoCaching()
        {
            // Arrange
            var fakeTargetMock = new Mock<IFakeTarget>(MockBehavior.Strict);
            fakeTargetMock
                .Setup(x => x.NotCachedMethod())
                .Returns(new object());
            var fakeCacheProviderMock = new Mock<ICacheProvider>(MockBehavior.Strict);
            var restrictingProvider = new CachingProvider(fakeCacheProviderMock.Object);
            var restrictedTarget = restrictingProvider.GetCachingTarget(fakeTargetMock.Object);

            // Act
            restrictedTarget.NotCachedMethod();

            // Assert
            fakeTargetMock.Verify(x => x.NotCachedMethod(), Times.Once);
            fakeCacheProviderMock.VerifyAll();
        }

        [Fact]
        public void GetCachingTarget_NotCachedMethodWithCustomAttribute_NoCaching()
        {
            // Arrange
            var fakeTargetMock = new Mock<IFakeTarget>(MockBehavior.Strict);
            fakeTargetMock
                .Setup(x => x.NotCachedMethodWithCustomAttribute())
                .Returns(new object());
            var fakeCacheProviderMock = new Mock<ICacheProvider>(MockBehavior.Strict);
            var restrictingProvider = new CachingProvider(fakeCacheProviderMock.Object);
            var restrictedTarget = restrictingProvider.GetCachingTarget(fakeTargetMock.Object);

            // Act
            restrictedTarget.NotCachedMethodWithCustomAttribute();

            // Assert
            fakeTargetMock.Verify(x => x.NotCachedMethodWithCustomAttribute(), Times.Once);
            fakeCacheProviderMock.VerifyAll();
        }

        [Fact]
        public void GetCachingTarget_CachedMethod_CacheProviderIsCalled()
        {
            // Arrange
            var fakeTargetMock = new Mock<IFakeTarget>(MockBehavior.Strict);
            fakeTargetMock
                .Setup(x => x.CachedMethod())
                .Returns(123);
            var fakeCacheProviderMock = new Mock<ICacheProvider>(MockBehavior.Loose);
            var restrictingProvider = new CachingProvider(fakeCacheProviderMock.Object);
            var restrictedTarget = restrictingProvider.GetCachingTarget(fakeTargetMock.Object);

            // Act
            restrictedTarget.CachedMethod();

            // Assert
            fakeTargetMock.Verify(x => x.CachedMethod(), Times.Once);
            fakeCacheProviderMock.Verify(x => x.GetValue(It.Is<string>(y => y.Equals("1457445245"))), Times.Once);
            fakeCacheProviderMock.Verify(
                x => x.SetValue(
                    It.Is<string>(y => y.Equals("1457445245")),
                    It.Is<string>(y => y.Equals("123")),
                    It.Is<TimeSpan>(y => y.Equals(TimeSpan.FromSeconds(1)))),
                Times.Once);
        }

        [Fact]
        public void GetCachingTarget_CachedMethodWithCustomAttribute_CacheProviderIsCalled()
        {
            // Arrange
            var fakeTargetMock = new Mock<IFakeTarget>(MockBehavior.Strict);
            fakeTargetMock
                .Setup(x => x.CachedMethodWithCustomAttribute())
                .Returns(123);
            var fakeCacheProviderMock = new Mock<ICacheProvider>(MockBehavior.Loose);
            var restrictingProvider = new CachingProvider(fakeCacheProviderMock.Object);
            var restrictedTarget = restrictingProvider.GetCachingTarget(fakeTargetMock.Object);

            // Act
            restrictedTarget.CachedMethodWithCustomAttribute();

            // Assert
            fakeTargetMock.Verify(x => x.CachedMethodWithCustomAttribute(), Times.Once);
            fakeCacheProviderMock.Verify(x => x.GetValue(It.Is<string>(y => y.Equals("-869456893"))), Times.Once);
            fakeCacheProviderMock.Verify(
                x => x.SetValue(
                    It.Is<string>(y => y.Equals("-869456893")),
                    It.Is<string>(y => y.Equals("123")),
                    It.Is<TimeSpan>(y => y.Equals(TimeSpan.FromSeconds(1)))),
                Times.Once);
        }
    }
}
