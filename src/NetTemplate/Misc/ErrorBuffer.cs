namespace NetTemplate.Misc;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using StringBuilder = System.Text.StringBuilder;

/** Used during tests to track all errors */
public class ErrorBuffer : ITemplateErrorListener
{
    private readonly List<TemplateMessage> errors = new List<TemplateMessage>();

    public ReadOnlyCollection<TemplateMessage> Errors
    {
        get
        {
            return errors.AsReadOnly();
        }
    }

    protected List<TemplateMessage> ErrorList
    {
        get
        {
            return errors;
        }
    }

    public virtual void CompiletimeError(TemplateMessage msg)
    {
        errors.Add(msg);
    }

    public virtual void RuntimeError(TemplateMessage msg)
    {
        // ignore these
        if (msg.Error != ErrorType.NO_SUCH_PROPERTY)
            errors.Add(msg);
    }

    public virtual void IOError(TemplateMessage msg)
    {
        errors.Add(msg);
    }

    public virtual void InternalError(TemplateMessage msg)
    {
        errors.Add(msg);
    }

    public override string ToString()
    {
        StringBuilder buf = new StringBuilder();
        foreach (TemplateMessage m in errors)
            buf.AppendLine(m.ToString());

        return buf.ToString();
    }
}
