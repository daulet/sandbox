using Castle.DynamicProxy;
using Echo.Core;

namespace Echo.UnitTesting
{
    public class TestPlayer
    {
        private readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly IInvocationReader _invocationReader;

        public TestPlayer(IEchoReader echoReader)
        {
            _invocationReader = new InvocationReader(echoReader);
        }

        public TestEntry GetEntryValues()
        {
            var arguments = _invocationReader.FindEntryArguments();
            return new TestEntry(arguments);
        }

        public TTarget GetReplayingTarget<TTarget>()
            where TTarget : class
        {
            var replayingInterceptor = new ReplayingInterceptor<TTarget>(_invocationReader);
            return _generator.CreateInterfaceProxyWithoutTarget<TTarget>(
#if DEBUG
                new RecordingInterceptor<TTarget>(new ConsoleWriter()),
#endif
                replayingInterceptor);
        }

        public void VerifyAll()
        {
            // verify all targets were hit in prerecorded order, with the same values
        }

        public void VerifyOrder()
        {
            // verify all targets were hit in prerecorded order
        }

        public void VerifyInvocations()
        {
            // verify all targets were hit
        }
    }
}
