namespace NetTemplate.Extensions;

using System;
#if !NETSTANDARD
    using BindingFlags = System.Reflection.BindingFlags;
    using MethodInfo = System.Reflection.MethodInfo;
#endif

public static class ExceptionExtensions
{
#if !NETSTANDARD
        private static readonly Action<Exception> _internalPreserveStackTrace = GetInternalPreserveStackTraceDelegate();

        private static Action<Exception> GetInternalPreserveStackTraceDelegate()
        {
            MethodInfo methodInfo = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);
            if (methodInfo == null)
                return null;

            return (Action<Exception>)Delegate.CreateDelegate(typeof(Action<Exception>), methodInfo);
        }
#endif

#pragma warning disable 618
    public static bool IsCritical(this Exception e)
    {
        if (e is OutOfMemoryException
            || e is BadImageFormatException)
        {
            return true;
        }

#if NETSTANDARD
        switch (e.GetType().FullName)
        {
            case "System.AccessViolationException":
            case "System.StackOverflowException":
            case "System.ExecutionEngineException":
            case "System.AppDomainUnloadedException":
                return true;

            default:
                break;
        }
#else
            if (e is AccessViolationException
                || e is StackOverflowException
                || e is ExecutionEngineException
                || e is AppDomainUnloadedException)
            {
                return true;
            }
#endif

        return false;
    }
#pragma warning restore 618

    public static void PreserveStackTrace(this Exception e)
    {
#if !NETSTANDARD
            if (_internalPreserveStackTrace != null)
                _internalPreserveStackTrace(e);
#endif
    }
}
