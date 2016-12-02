using Castle.DynamicProxy;
using Echo.Utilities;
using System;
using System.Reflection;

namespace Echo.Core
{
    internal class ReplayingInterceptor<TTarget> : IInterceptor
        where TTarget : class
    {
        private readonly IInvocationReader _invocationReader;

        internal ReplayingInterceptor(IInvocationReader invocationReader)
        {
            _invocationReader = invocationReader;
        }

        // TODO doesn't handle async results
        public void Intercept(IInvocation invocation)
        {
            try
            {
                var returnValue = FindInvocationResult(invocation.Method, invocation.Arguments);
                if (returnValue is ExceptionInvocationResult)
                {
                    throw (returnValue as ExceptionInvocationResult).ThrownException;
                }
                else if (returnValue is ValueInvocationResult)
                {
                    invocation.ReturnValue = (returnValue as ValueInvocationResult).ReturnedValue;
                }
                else if (returnValue is VoidInvocationResult)
                {
                    // no ReturnValue in this case
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            catch (NoEchoFoundException)
            {
                // TODO This behaviour needs to be configurable: return default or throw - should be parameter of the constructor
                // TODO does null work for value types?
                invocation.ReturnValue = Activator.CreateInstance(invocation.Method.ReturnType);

            }
        }

        private InvocationResult FindInvocationResult(MethodInfo methodInfo, object[] arguments)
        {
            var recordedInvocations = _invocationReader.GetAllInvocations();
            foreach (var invocation in recordedInvocations)
            {
                // TODO test case when method signatures match on different interfaces
                if (invocation.Target == typeof(TTarget))
                {
                    if (string.Equals(invocation.Method, methodInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        if (InvocationUtility.IsArgumentListMatch(invocation.Arguments, arguments))
                        {
                            return invocation.Result;
                        }
                    }
                }
            }
            throw new NoEchoFoundException();
        }
    }
}
