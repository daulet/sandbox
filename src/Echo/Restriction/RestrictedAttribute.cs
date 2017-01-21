using System;

namespace Echo.Restriction
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class RestrictedAttribute : Attribute
    {
    }
}
