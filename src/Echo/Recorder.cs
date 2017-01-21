using Castle.DynamicProxy;
using Echo.Core;
using System;

namespace Echo
{
    public class Recorder
    {
        private readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly IInvocationListener _invocationListener;

        public Recorder(IEchoWriter echoWriter)
            : this(new InvocationSerializer(echoWriter))
        {
        }

        internal Recorder(IInvocationListener invocationListener)
        {
            _invocationListener = invocationListener;
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

            var recordingInterceptor = new ListeningInterceptor<TTarget>(_invocationListener);
            return _generator.CreateInterfaceProxyWithTarget<TTarget>(target,
                new ListeningInterceptor<TTarget>(InstancePool.LoggingListener),
                recordingInterceptor);
        }
    }
}
