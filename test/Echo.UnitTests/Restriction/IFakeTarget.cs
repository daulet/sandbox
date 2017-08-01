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
    }
}