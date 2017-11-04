using Echo.Core;
using System;
using System.IO;

namespace Echo
{
    public class Recorder
    {
        private readonly IInvocationListener _invocationListener;

        public Recorder(TextWriter writer)
            : this(new InvocationSerializer(writer))
        {
        }

        internal Recorder(IInvocationListener invocationListener)
        {
            _invocationListener = invocationListener;
        }

        // target is not injected in constructor since there could be multiple targets per recording
        public TTarget GetRecordingTarget<TTarget>(TTarget target)
            where TTarget : class
        {
            // only public interface recording is supported
            var targetType = typeof(TTarget);
            if (!targetType.IsInterface || !targetType.IsPublic)
            {
                throw new NotSupportedException();
            }

            return InstancePool.ProxyGenerator
                .CreateInterfaceProxyWithTarget<TTarget>(target,
                    new ListeningInterceptor<TTarget>(InstancePool.LoggingListener),
                    new ListeningInterceptor<TTarget>(_invocationListener));
        }
    }
}
