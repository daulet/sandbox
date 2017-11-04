using Echo.Core;
using System;
using System.IO;

namespace Echo
{
    public class Player : IDisposable
    {
        private readonly IInvocationReader _invocationReader;
        private TextReader _textReader;

        public Player(TextReader textReader)
            : this(new InvocationDeserializer(textReader))
        {
            _textReader = textReader;
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
