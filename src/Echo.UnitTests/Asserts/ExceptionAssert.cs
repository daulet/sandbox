using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Echo.UnitTests.Asserts
{
    public static class ExceptionAssert
    {
        public static void Throws<T>(Action action)
            where T : Exception
        {
            try
            {
                action();
            }
            catch (T ex)
            {
                Assert.IsTrue(ex is T, $"Expecting {typeof(T)}, but action threw {ex.GetType()}");
                return;
            }

            Assert.Fail($"Action is expected to throw {typeof(T)}");
        }

        public static async Task ThrowsAsync<T>(Func<Task> action)
            where T : Exception
        {
            try
            {
                await action();
            }
            catch (T ex)
            {
                Assert.IsTrue(ex is T, $"Expecting {typeof(T)}, but action threw {ex.GetType()}");
                return;
            }

            Assert.Fail($"Action is expected to throw {typeof(T)}");
        }
    }
}
