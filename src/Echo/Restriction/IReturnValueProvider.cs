using System;

namespace Echo.Restriction
{
    public interface IReturnValueProvider
    {
        object GetReturnValue(Type returnType);
    }
}
