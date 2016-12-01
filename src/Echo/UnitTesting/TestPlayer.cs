using Castle.DynamicProxy;
using Echo.Core;

namespace Echo.UnitTesting
{
    public class TestPlayer
    {
        private readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly IInvocationReader _invocationReader;
        private readonly IValidationReader _validationReader;

        public TestPlayer(IEchoReader echoReader)
        {
            var reader = new ValidationReader(echoReader);
            _invocationReader = reader;
            _validationReader = reader;
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

        public TestEntry GetTestEntry()
        {
            var arguments = _validationReader.FindEntryArguments();
            return new TestEntry(arguments);
        }

        public void VerifyAll()
        {
            // verify all targets were hit in prerecorded order, with the same values
            _validationReader.VerifyAll();
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
