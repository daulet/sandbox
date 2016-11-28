using Castle.DynamicProxy;
using System;

namespace Echo.Core
{
    internal class RecordingInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"Intercepted argument {invocation.Arguments[0]}");

            invocation.Proceed();

            Console.WriteLine($"Intercepted output {invocation.ReturnValue}");
        }
    }
}
