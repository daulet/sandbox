using Echo.Core;
using System;
using System.IO;
using System.Reflection;

namespace Echo
{
    public class Recorder : IDisposable
    {
        private readonly IInvocationListener _invocationListener;
        private TextWriter _textWriter;

        public Recorder(TextWriter writer)
            : this(new InvocationSerializer(writer))
        {
            _textWriter = writer;
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
            if (!targetType.GetTypeInfo().IsInterface || !targetType.GetTypeInfo().IsPublic)
            {
                throw new NotSupportedException();
            }

            return InstancePool.ProxyGenerator
                .CreateInterfaceProxyWithTarget<TTarget>(target,
                    new ListeningInterceptor<TTarget>(InstancePool.LoggingListener),
                    new ListeningInterceptor<TTarget>(_invocationListener));
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
                _textWriter?.Dispose();
                _textWriter = null;
            }
        }

        #endregion
    }
}
