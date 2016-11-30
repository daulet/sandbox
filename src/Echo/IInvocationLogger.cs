namespace Echo
{
    public interface IInvocationLogger
    {
        void WriteSerializedInvocation(string serializedInvocation);
    }
}
