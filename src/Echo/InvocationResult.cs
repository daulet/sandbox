using System;

namespace Echo
{
    public abstract class InvocationResult
    {
        public abstract Type GetResultType();

        public static VoidInvocationResult Void => VoidInvocationResult.Instance;
    }

    public sealed class ExceptionInvocationResult : InvocationResult
    {
        public Exception ThrownException { get; }

        public override Type GetResultType()
        {
            return ThrownException.GetType();
        }

        public override string ToString()
        {
            return ThrownException.ToString();
        }

        internal ExceptionInvocationResult(Exception thrownException)
        {
            ThrownException = thrownException;
        }
    }

    public sealed class ValueInvocationResult : InvocationResult
    {
        public object ReturnValue { get; }

        public override Type GetResultType()
        {
            return ReturnValue.GetType();
        }

        public override string ToString()
        {
            return ReturnValue?.ToString();
        }

        internal ValueInvocationResult(object returnValue)
        {
            ReturnValue = returnValue;
        }
    }

    public sealed class VoidInvocationResult : InvocationResult
    {
        private VoidInvocationResult()
        {
            // a single instance of the class is ever used
        }

        public override Type GetResultType()
        {
            return typeof(void);
        }

        public override string ToString()
        {
            return "(void)";
        }

        public static VoidInvocationResult Instance { get; } = new VoidInvocationResult();
    }
}
