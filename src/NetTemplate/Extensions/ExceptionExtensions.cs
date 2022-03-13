namespace NetTemplate.Extensions;

using System;

public static class ExceptionExtensions
{
#pragma warning disable 618
    public static bool IsCritical(this Exception e)
    {
        if (e is OutOfMemoryException
            || e is BadImageFormatException)
        {
            return true;
        }

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

        return false;
    }
#pragma warning restore 618

    public static void PreserveStackTrace(this Exception e)
    {
    }
}
