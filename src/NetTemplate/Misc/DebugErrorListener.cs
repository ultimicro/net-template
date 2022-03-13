namespace NetTemplate.Misc;

using Exception = System.Exception;

public class DebugErrorListener : ITemplateErrorListener
{
    public virtual void CompiletimeError(TemplateMessage msg)
    {
        System.Diagnostics.Debug.WriteLine(msg);
    }

    public virtual void RuntimeError(TemplateMessage msg)
    {
        if (msg.Error != ErrorType.NO_SUCH_PROPERTY)
            System.Diagnostics.Debug.WriteLine(msg);
    }

    public virtual void IOError(TemplateMessage msg)
    {
        System.Diagnostics.Debug.WriteLine(msg);
    }

    public virtual void InternalError(TemplateMessage msg)
    {
        System.Diagnostics.Debug.WriteLine(msg);
    }

    public virtual void Error(string s)
    {
        Error(s, null);
    }

    public virtual void Error(string s, Exception e)
    {
        System.Diagnostics.Debug.WriteLine(s);
        if (e != null)
            System.Diagnostics.Debug.WriteLine(e.StackTrace);
    }
}
