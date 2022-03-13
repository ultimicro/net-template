namespace NetTemplate.Compiler;

using Exception = System.Exception;

public class TemplateException : Exception
{
    public TemplateException()
    {
    }

    public TemplateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
