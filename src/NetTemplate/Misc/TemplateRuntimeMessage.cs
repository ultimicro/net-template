namespace NetTemplate.Misc;

using Exception = System.Exception;
using StringBuilder = System.Text.StringBuilder;

/** Used to track errors that occur in the Template interpreter. */
public class TemplateRuntimeMessage : TemplateMessage
{
    /** Where error occurred in bytecode memory */
    private readonly int ip = -1;

    private readonly TemplateFrame _frame;

    public TemplateRuntimeMessage(ErrorType error, int ip)
        : this(error, ip, null)
    {
    }

    public TemplateRuntimeMessage(ErrorType error, int ip, TemplateFrame frame)
        : this(error, ip, frame, null)
    {
    }

    public TemplateRuntimeMessage(ErrorType error, int ip, TemplateFrame frame, object arg)
        : this(error, ip, frame, null, arg, null)
    {
    }

    public TemplateRuntimeMessage(ErrorType error, int ip, TemplateFrame frame, Exception e, object arg)
        : this(error, ip, frame, e, arg, null)
    {
    }

    public TemplateRuntimeMessage(ErrorType error, int ip, TemplateFrame frame, Exception e, object arg, object arg2)
        : base(error, frame?.Template, e, arg, arg2)
    {
        this.ip = ip;
        this._frame = frame;
    }

    public TemplateRuntimeMessage(ErrorType error, int ip, TemplateFrame frame, Exception e, object arg, object arg2, object arg3)
        : base(error, frame?.Template, e, arg, arg2, arg3)
    {
        this.ip = ip;
        this._frame = frame;
    }

    public TemplateFrame Frame
    {
        get
        {
            return _frame;
        }
    }

    public Interval SourceInterval
    {
        get
        {
            if (ip < 0 || Self == null || Self.impl == null || Self.impl.sourceMap == null || ip >= Self.impl.sourceMap.Length)
                return null;

            return Self.impl.sourceMap[ip];
        }
    }

    /** Given an ip (code location), get it's range in source template then
     *  return it's template line:col.
     */
    public virtual string GetSourceLocation()
    {
        Interval interval = SourceInterval;
        if (interval == null)
            return null;

        // get left edge and get line/col
        int i = interval.Start;
        Coordinate loc = Utility.GetLineCharPosition(Self.impl.Template, i);
        return loc.ToString();
    }

    public override string ToString()
    {
        StringBuilder buf = new StringBuilder();
        string loc = null;
        if (_frame != null)
        {
            loc = GetSourceLocation();
            buf.Append("context [");
            buf.Append(_frame.GetEnclosingInstanceStackString());
            buf.Append("]");
        }
        if (loc != null)
            buf.Append(" " + loc);
        buf.Append(" " + base.ToString());
        return buf.ToString();
    }
}
