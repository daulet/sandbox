using Castle.DynamicProxy;
using Echo.Core;
using System;

namespace Echo
{
    public class Recorder
    {
        private readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly IInvocationWriter _invocationWriter;

        public Recorder(IEchoWriter echoWriter)
            : this(new InvocationWriter(echoWriter))
        {
        }

        internal Recorder(IInvocationWriter invocationWriter)
        {
            _invocationWriter = invocationWriter;
        }

        public TTarget GetRecordingTarget<TTarget>(TTarget target)
            where TTarget : class
        {
            // only public interface recording is supported
            var targetType = typeof(TTarget);
            if (!targetType.IsInterface || !targetType.IsPublic)
            {
                throw new NotSupportedException();
            }

            var recordingInterceptor = new RecordingInterceptor(_invocationWriter);
            return _generator.CreateInterfaceProxyWithTarget<TTarget>(target,
#if DEBUG
                new RecordingInterceptor(new ConsoleWriter()),
#endif
                recordingInterceptor);
        }
    }
}
