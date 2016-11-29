using System;
using System.Reflection;

namespace Echo.Core
{
    internal class ConsoleWritter : IInvocationWritter
    {
        public void RecordInvocation(MethodInfo methodInfo, InvocationResult invocationResult, object[] arguments)
        {
            Console.WriteLine($"Intercepting {methodInfo.Name} method:");
            foreach (var argument in arguments)
            {
                Console.WriteLine($"\tArgument {argument?.GetType()}: {argument}");
            }
            Console.WriteLine($"\tReturn {invocationResult?.GetResultType()}: {invocationResult}");
        }
    }
}
