using System.IO;
using Echo.Core;

namespace Echo.UnitTesting
{
    public class TestPlayer
    {
        private readonly IInvocationReader _invocationReader;
        private readonly ValidatingListener _validatingListener;

        public TestPlayer(TextReader reader)
        {
            _invocationReader = new InvocationDeserializer(reader);
            _validatingListener = new ValidatingListener(_invocationReader);
        }

        public TTarget GetReplayingTarget<TTarget>()
            where TTarget : class
        {
            return InstancePool.ProxyGenerator
                .CreateInterfaceProxyWithoutTarget<TTarget>(
                    new ListeningInterceptor<TTarget>(InstancePool.LoggingListener),
                    new ListeningInterceptor<TTarget>(_validatingListener),
                    new ReplayingInterceptor<TTarget>(_invocationReader));
        }

        public TestEntry GetTestEntry()
        {
            var arguments = _invocationReader.FindEntryArguments();
            return new TestEntry(arguments);
        }

        public TTarget GetTestEntryTarget<TTarget>(TTarget target)
            where TTarget : class
        {
            return InstancePool.ProxyGenerator
                .CreateInterfaceProxyWithTarget<TTarget>(target,
                    new ListeningInterceptor<TTarget>(InstancePool.LoggingListener),
                    new ListeningInterceptor<TTarget>(_validatingListener));
        }

        #region Validation

        public void VerifyAll()
        {
            try
            {
                // verify all targets were hit in prerecorded order, with the same values
                _validatingListener.VerifyAll();
            }
            catch (EchoVerificationException)
            {
                throw;
            }
        }

        /// <summary>
        /// Verify that all mocked dependencies are hit.
        /// The same as <see cref="VerifyAll"/> but doesn't verify entry point
        /// </summary>
        public void VerifyMocks()
        {
            // TODO implement
        }

        public void VerifyOrder()
        {
            // TODO verify all targets were hit in prerecorded order
        }

        public void VerifyInvocations()
        {
            // TODO verify all targets were hit
        }

        #endregion
    }
}
