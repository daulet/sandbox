using Castle.DynamicProxy;
using Echo.Core;

namespace Echo
{
    public class Player
    {
        private readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly IInvocationReader _invocationReader;

        public Player(IEchoReader echoReader)
            : this(new InvocationReader(echoReader))
        {
        }

        internal Player(IInvocationReader invocationReader)
        {
            _invocationReader = invocationReader;
        }

        public TTarget GetReplayingTarget<TTarget>()
            where TTarget : class
        {
            var replayingInterceptor = new ReplayingInterceptor(_invocationReader);
            return _generator.CreateInterfaceProxyWithoutTarget<TTarget>(
#if DEBUG
                new RecordingInterceptor(new ConsoleWriter()),
#endif
                replayingInterceptor);
        }
    }
}
