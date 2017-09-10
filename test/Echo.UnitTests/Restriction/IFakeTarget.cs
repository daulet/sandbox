using Echo.Restriction;

namespace Echo.UnitTests.Restriction
{
    public interface IFakeTarget
    {
        void UnrestrictedMethod();

        [Fake]
        void UnrestrictedMethodWithCustomAttribute();

        [Restricted]
        void RestrictedMethod();

        [Fake]
        [Restricted]
        void RestrictedMethodWithCustomAttribute();

        [Restricted]
        T RestrictedMethodWithObjectReturnValue<T>()
            where T : class;

        [Restricted]
        void RestrictedMethodWithParameter<T>(T obj);

        [Restricted]
        T RestrictedMethodWithStructReturnValue<T>()
            where T : struct;
    }
}
