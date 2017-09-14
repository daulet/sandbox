using System;
using Echo.Restriction;

namespace Echo.UnitTests.Restriction
{
    public class FakeReturnValueProvider : IReturnValueProvider
    {
        public object GetReturnValue(Type returnType)
        {
            if (returnType == typeof(int)) {
                return (object)5;
            }
            return null;
        }
    }
}
