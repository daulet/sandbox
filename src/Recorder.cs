using Castle.DynamicProxy;
using Echo.Core;

namespace Echo
{
    public class Recorder
    {
        private readonly ProxyGenerator _generator = new ProxyGenerator();

        public TTarget GetRecordingTarget<TTarget>(TTarget target)
            where TTarget : class
        {
            // TODO test if TTarget is interface
            // TODO are we intercepting throws?
            // TODO how well does this work with async?

            var loggingInterceptor = new LoggingInterceptor(new ConsoleWritter());
            return _generator.CreateInterfaceProxyWithTarget<TTarget>(target, loggingInterceptor);
        }
    }
}
