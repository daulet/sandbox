using Castle.DynamicProxy;
using Echo.Core;

namespace Echo
{
    // TODO add IFluentInterface
    // TODO add tests
    // TODO can we generate code based on echo?
    public class Player
    {
        private readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly IInvocationReader _invocationReader;

        public Player(IEchoReader echoReader)
            : this(new InvocationDeserializer(echoReader))
        {
        }

        internal Player(IInvocationReader invocationReader)
        {
            _invocationReader = invocationReader;
        }

        public TTarget GetReplayingTarget<TTarget>()
            where TTarget : class
        {
            var replayingInterceptor = new ReplayingInterceptor<TTarget>(_invocationReader);
            return _generator.CreateInterfaceProxyWithoutTarget<TTarget>(
#if DEBUG
                new ListeningInterceptor<TTarget>(new DebugListener()),
#endif
                replayingInterceptor);
        }
    }
}
