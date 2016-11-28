using System;
using System.Reflection;

namespace Echo.Core
{
    internal class ConsoleWritter : ITapeWritter
    {
        public void RecordInvocation(MethodInfo methodInfo, object returnValue, object[] arguments)
        {
            Console.WriteLine($"Intercepting {methodInfo.Name} method:");
            foreach (var argument in arguments)
            {
                Console.WriteLine($"\tArgument {argument.GetType()}: {argument}");
            }
            Console.WriteLine($"\tReturn {returnValue.GetType()}: {returnValue}");
        }
    }
}
