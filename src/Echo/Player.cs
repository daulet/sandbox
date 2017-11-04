using Echo.Core;
using System;
using System.IO;

namespace Echo
{
    public class Player
    {
        private readonly IInvocationReader _invocationReader;

        public Player(TextReader reader)
            : this(new InvocationDeserializer(reader))
        {
        }

        internal Player(IInvocationReader invocationReader)
        {
            _invocationReader = invocationReader;
        }

        // This class is not generic since there could be multiple targets per recording
        public TTarget GetReplayingTarget<TTarget>()
            where TTarget : class
        {
            // only public interface replaying is supported
            var targetType = typeof(TTarget);
            if (!targetType.IsInterface || !targetType.IsPublic)
            {
                throw new NotSupportedException();
            }

            return InstancePool.ProxyGenerator
                .CreateInterfaceProxyWithoutTarget<TTarget>(
                    new ListeningInterceptor<TTarget>(InstancePool.LoggingListener),
                    new ReplayingInterceptor<TTarget>(_invocationReader));
        }
    }
}
