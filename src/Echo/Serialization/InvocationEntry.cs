using Echo.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Echo.Serialization
{
    // TODO rename to InvocationEntity. Entry is used to refer to first invocation
    internal class InvocationEntry
    {
        // TODO test for setters in all properties

        public object[] Arguments { get; set; }

        public string Method { get; set; }

        public InvocationResultType ResultType { get; set; }

        public object ReturnValue { get; set; }

        public string Target { get; set; }

        // TODO test for parameterless constructor
        public InvocationEntry()
        {
        }

        public InvocationEntry(Type targetType, MethodInfo methodInfo, object[] arguments, InvocationResult invocationResult)
        {
            Arguments = arguments;
            InvocationResult = invocationResult;
            Method = methodInfo.Name;
            TargetType = targetType;
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
                            var exceptionType = Type.GetType(ReturnValue as string);
                            var exceptionInstance = Activator.CreateInstance(exceptionType);
                            _invocationResult = new ExceptionInvocationResult(exceptionInstance as Exception);
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
            private set
            {
                _invocationResult = value;

                if (value is ExceptionInvocationResult)
                {
                    ResultType = InvocationResultType.Exception;
                    // in case of exception just serialize its type
                    // TODO serialize all public custom properties of exception in case user cares about them
                    ReturnValue = (value as ExceptionInvocationResult).ThrownException.GetType().AssemblyQualifiedName;
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

        internal Type TargetType
        {
            get
            {
                if (_targetType == null)
                {
                    var matchingType = Type.GetType(Target);
                    if (matchingType == null)
                    {
                        throw new KeyNotFoundException();
                    }
                    else
                    {
                        _targetType = matchingType;
                    }
                }
                return _targetType;
            }
            set
            {
                _targetType = value;
                Target = _targetType.AssemblyQualifiedName;
            }
        }
        private Type _targetType;

        #endregion
    }
}
