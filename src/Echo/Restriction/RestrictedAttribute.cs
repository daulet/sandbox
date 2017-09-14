using System;

namespace Echo.Restriction
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class RestrictedAttribute : Attribute
    {
        public Type ProviderType { get; }

        /// <summary>
        /// Restricted calls will return default value of that type.
        /// </summary>
        /// <remarks>
        /// Use the other constructor to customize return value.
        /// </remarks>
        public RestrictedAttribute()
        {
        }

        /// <summary>
        /// Customize returned value for restricted calls.
        /// </summary>
        /// <param name="providerType">Must be type that implements <see cref="IReturnValueProvider"/></param>
        public RestrictedAttribute(Type providerType)
        {
            ProviderType = providerType;
        }
    }
}
