namespace NetTemplate.Misc;

using Exception = System.Exception;
using IToken = Antlr.Runtime.IToken;
using StringWriter = System.IO.StringWriter;

/** Upon error, Template creates an TemplateMessage or subclass instance and notifies
 *  the listener.  This root class is used for IO and internal errors.
 *
 *  @see TemplateRuntimeMessage
 *  @see TemplateCompiletimeMessage
 */
public class TemplateMessage
{
    /** if in debug mode, has created instance, Add attr events and eval
     *  template events.
     */
    private readonly Template self;
    private readonly ErrorType error;
    private readonly object arg;
    private readonly object arg2;
    private readonly object arg3;
    private readonly Exception cause;

    public TemplateMessage(ErrorType error)
    {
        this.error = error;
    }

    public TemplateMessage(ErrorType error, Template self)
        : this(error)
    {
        this.self = self;
    }

    public TemplateMessage(ErrorType error, Template self, Exception cause)
        : this(error, self)
    {
        this.cause = cause;
    }

    public TemplateMessage(ErrorType error, Template self, Exception cause, object arg)
        : this(error, self, cause)
    {
        this.arg = arg;
    }

    public TemplateMessage(ErrorType error, Template self, Exception cause, IToken where, object arg)
        : this(error, self, cause, where)
    {
        this.arg = arg;
    }

    public TemplateMessage(ErrorType error, Template self, Exception cause, object arg, object arg2)
        : this(error, self, cause, arg)
    {
        this.arg2 = arg2;
    }

    public TemplateMessage(ErrorType error, Template self, Exception cause, object arg, object arg2, object arg3)
        : this(error, self, cause, arg, arg2)
    {
        this.arg3 = arg3;
    }

    public Template Self
    {
        get
        {
            return self;
        }
    }

    public ErrorType Error
    {
        get
        {
            return error;
        }
    }

    public object Arg
    {
        get
        {
            return arg;
        }
    }

    public object Arg2
    {
        get
        {
            return arg2;
        }
    }

    public object Arg3
    {
        get
        {
            return arg3;
        }
    }

    public Exception Cause
    {
        get
        {
            return cause;
        }
    }

    public override string ToString()
    {
        StringWriter sw = new StringWriter();
        string msg = string.Format(error.Message, arg, arg2, arg3);
        sw.Write(msg);
        if (cause != null)
        {
            sw.WriteLine();
            sw.Write("Caused by: ");
            sw.WriteLine(cause.Message);
            sw.Write(cause.StackTrace);
        }
        return sw.ToString();
    }
}
