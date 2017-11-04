using System;
using System.IO;
using Echo.Core;

namespace Echo.UnitTesting
{
    public class TestPlayer : IDisposable
    {
        private readonly IInvocationReader _invocationReader;
        private TextReader _textReader;
        private readonly ValidatingListener _validatingListener;

        public TestPlayer(TextReader textReader)
        {
            _invocationReader = new InvocationDeserializer(textReader);
            _textReader = textReader;
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

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textReader?.Dispose();
                _textReader = null;
            }
        }

        #endregion
    }
}
