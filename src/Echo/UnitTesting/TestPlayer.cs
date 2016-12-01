using Castle.DynamicProxy;
using Echo.Core;

namespace Echo.UnitTesting
{
    public class TestPlayer
    {
        private readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly IInvocationReader _invocationReader;
        private readonly ValidatingListener _validatingListener;

        public TestPlayer(IEchoReader echoReader)
        {
            _invocationReader = new InvocationDeserializer(echoReader);
            _validatingListener = new ValidatingListener(_invocationReader);
        }

        public TTarget GetReplayingTarget<TTarget>()
            where TTarget : class
        {
            var replayingInterceptor = new ReplayingInterceptor<TTarget>(_invocationReader);
            var validatingInterceptor = new ListeningInterceptor<TTarget>(_validatingListener);
            return _generator.CreateInterfaceProxyWithoutTarget<TTarget>(
#if DEBUG
                new ListeningInterceptor<TTarget>(new DebugListener()),
#endif
                validatingInterceptor,
                replayingInterceptor);
        }

        public TestEntry GetTestEntry()
        {
            var arguments = _invocationReader.FindEntryArguments();
            return new TestEntry(arguments);
        }

        public TTarget GetTestEntryTarget<TTarget>(TTarget target)
            where TTarget : class
        {
            var validatingInterceptor = new ListeningInterceptor<TTarget>(_validatingListener);
            return _generator.CreateInterfaceProxyWithTarget<TTarget>(target,
#if DEBUG
                new ListeningInterceptor<TTarget>(new DebugListener()),
#endif
                validatingInterceptor);
        }

        #region Validation

        public void VerifyAll()
        {
            // verify all targets were hit in prerecorded order, with the same values
            _validatingListener.VerifyAll();
        }

        /// <summary>
        /// Verify that all mocked dependencies are hit.
        /// The same as <see cref="VerifyAll"/> but doesn't verify entry point
        /// </summary>
        public void VerifyMocks()
        {

        }

        public void VerifyOrder()
        {
            // verify all targets were hit in prerecorded order
        }

        public void VerifyInvocations()
        {
            // verify all targets were hit
        }

        #endregion
    }
}
