using System;
using System.IO;
using Echo.UnitTests.Fakes;
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
                Assert.False(typeof(IInternalFakeTarget).IsPublic, "Interface can't be public for test to be valid");

                // Assert
                Assert.Throws<NotSupportedException>(() =>
                    // Act
                    recorder.GetReplayingTarget<IInternalFakeTarget>());
            }
        }
    }
}
