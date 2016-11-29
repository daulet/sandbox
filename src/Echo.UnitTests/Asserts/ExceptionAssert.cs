using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Echo.UnitTests.Asserts
{
    public static class ExceptionAssert
    {
        public static void Throws(Action action)
        {
            Throws<Exception>(action);
        }

        public static void Throws<T>(Action action)
            where T : Exception
        {
            try
            {
                action();

                Assert.Fail($"Action is expected to throw {typeof(T)}");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is T, $"Expecting {typeof(T)}, but action threw {ex.GetType()}");
            }
        }
    }
}
