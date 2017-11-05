using System;
using System.Runtime.Serialization;

namespace Echo.Core
{
    [Serializable]
    internal class NoEchoFoundException : Exception
    {
    }
}
