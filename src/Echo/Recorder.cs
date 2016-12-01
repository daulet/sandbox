using Castle.DynamicProxy;
using Echo.Core;
using System;

namespace Echo
{
    public class Recorder
    {
        private readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly IInvocationListener _invocationWriter;

        public Recorder(IEchoWriter echoWriter)
            : this(new InvocationSerializer(echoWriter))
        {
        }

        internal Recorder(IInvocationListener invocationWriter)
        {
            _invocationWriter = invocationWriter;
        }

        public TTarget GetRecordingTarget<TTarget>(TTarget target)
            where TTarget : class
        {
            // only public interface recording is supported
            var targetType = typeof(TTarget);
            if (!targetType.IsInterface || !targetType.IsPublic)
            {
                throw new NotSupportedException();
            }

            var recordingInterceptor = new ListeningInterceptor<TTarget>(_invocationWriter);
            return _generator.CreateInterfaceProxyWithTarget<TTarget>(target,
#if DEBUG
                new ListeningInterceptor<TTarget>(new DebugListener()),
#endif
                recordingInterceptor);
        }
    }
}
