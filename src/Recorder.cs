using Castle.DynamicProxy;
using Echo.Core;
using System;

namespace Echo
{
    public class Recorder
    {
        private readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly IInvocationWritter _invocationWritter;

        public Recorder(IInvocationWritter invocationWritter)
        {
            _invocationWritter = invocationWritter;
        }

        public TTarget GetRecordingTarget<TTarget>(TTarget target)
            where TTarget : class
        {
            // only interface intercepting and recording is supported
            if (!typeof(TTarget).IsInterface)
            {
                throw new NotSupportedException();
            }

            // TODO are we intercepting throws?
            // TODO what if TTarget is not public?
            // TODO how well does this work with async?

            var recordingInterceptor = new RecordingInterceptor(_invocationWritter);
            return _generator.CreateInterfaceProxyWithTarget<TTarget>(target,
#if DEBUG
                new RecordingInterceptor(new ConsoleWritter()),
#endif
                recordingInterceptor);
        }
    }
}
