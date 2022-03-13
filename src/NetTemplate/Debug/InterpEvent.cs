namespace NetTemplate.Debug;

using NetTemplate.Misc;
using ArgumentNullException = System.ArgumentNullException;

public class InterpEvent
{
    private readonly TemplateFrame _frame;
    // output location
    private readonly Interval _interval;

    public InterpEvent(TemplateFrame frame, Interval interval)
    {
        if (frame == null)
            throw new ArgumentNullException("frame");
        if (interval == null)
            throw new ArgumentNullException("interval");

        this._frame = frame;
        this._interval = interval;
    }

    public TemplateFrame Frame
    {
        get
        {
            return _frame;
        }
    }

    public Template Template
    {
        get
        {
            return _frame.Template;
        }
    }

    public Interval OutputInterval
    {
        get
        {
            return _interval;
        }
    }

    public override string ToString()
    {
        return string.Format("{0}{{self={1}, output={2}}}", GetType().Name, Template, OutputInterval);
    }
}
