namespace NetTemplate.Misc;

using ArgumentNullException = System.ArgumentNullException;
using Exception = System.Exception;
using TextWriter = System.IO.TextWriter;

public class TextWriterErrorListener : ITemplateErrorListener
{
    private readonly TextWriter _writer;

    public TextWriterErrorListener(TextWriter writer)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        _writer = writer;
    }

    public virtual void CompiletimeError(TemplateMessage msg)
    {
        _writer.WriteLine(msg);
    }

    public virtual void RuntimeError(TemplateMessage msg)
    {
        if (msg.Error != ErrorType.NO_SUCH_PROPERTY)
        {
            // ignore these
            _writer.WriteLine(msg);
        }
    }

    public virtual void IOError(TemplateMessage msg)
    {
        _writer.WriteLine(msg);
    }

    public virtual void InternalError(TemplateMessage msg)
    {
        _writer.WriteLine(msg);
        // throw new Error("internal error", msg.cause);
    }

    public virtual void Error(string s)
    {
        Error(s, null);
    }

    public virtual void Error(string s, Exception e)
    {
        _writer.WriteLine(s);
        if (e != null)
            _writer.WriteLine(e.StackTrace);
    }
}
