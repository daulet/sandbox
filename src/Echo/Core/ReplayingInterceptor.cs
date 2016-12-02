using Castle.DynamicProxy;
using Echo.Utilities;
using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

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
                    if (invocation.Method.ReturnType != invocation.ReturnValue.GetType() && invocation.ReturnValue.GetType() == typeof(JObject))
                    {
                        //var deserializeObjectMethods =
                        //    typeof(JToken)
                        //        .GetMethods(BindingFlags.Public).Where(x => x.Name == "ToObject" && x.IsGenericMethod).Where(method => method.GetParameters().Length == 0).First(m => m.GetParameters().First().ParameterType == typeof(string));

                        var deserializeObjectMethods =
                            typeof(JObject).GetMethods().Where(x => x.Name == "ToObject" && x.IsGenericMethod).First(method => method.GetParameters().Length == 0);

                        var genericDeserializerMethod = deserializeObjectMethods.MakeGenericMethod(invocation.Method.ReturnType);
                        invocation.ReturnValue = genericDeserializerMethod.Invoke(invocation.ReturnValue, null);
                    }

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
                // TODO This behaviour needs to be configurable: return null or throw
                // TODO does null work for value types?
                throw;
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
