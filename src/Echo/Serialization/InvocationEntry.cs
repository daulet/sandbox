using Echo.Core;
using System;
using System.Reflection;

namespace Echo.Serialization
{
    internal class InvocationEntry
    {
        // TODO test for setters in all properties

        public object[] Arguments { get; set; }

        public string Method { get; set; }

        public InvocationResultType ResultType { get; set; }

        public object ReturnValue { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        // TODO test for parameterless constructor
        public InvocationEntry()
        {
        }

        public InvocationEntry(DateTimeOffset timestamp, object[] arguments, MethodInfo methodInfo, InvocationResult invocationResult)
        {
            Arguments = arguments;
            InvocationResult = invocationResult;
            Method = methodInfo.Name;
            Timestamp = timestamp;
        }

        #region Internal

        internal InvocationResult InvocationResult
        {
            get
            {
                if (_invocationResult == null)
                {
                    switch (ResultType)
                    {
                        case InvocationResultType.Exception:
                            _invocationResult = new ExceptionInvocationResult(ReturnValue as Exception);
                            break;

                        case InvocationResultType.Value:
                            _invocationResult = new ValueInvocationResult(ReturnValue);
                            break;

                        case InvocationResultType.Void:
                            _invocationResult = InvocationResult.Void;
                            break;

                        default:
                            throw new NotSupportedException();
                    }
                }
                return _invocationResult;
            }
            set
            {
                _invocationResult = value;

                if (value is ExceptionInvocationResult)
                {
                    ResultType = InvocationResultType.Exception;
                    ReturnValue = (value as ExceptionInvocationResult).ThrownException;
                }
                else if (value is ValueInvocationResult)
                {
                    ResultType = InvocationResultType.Value;
                    ReturnValue = (value as ValueInvocationResult).ReturnedValue;
                }
                else if (value is VoidInvocationResult)
                {
                    ResultType = InvocationResultType.Void;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
        private InvocationResult _invocationResult;

        #endregion
    }
}
