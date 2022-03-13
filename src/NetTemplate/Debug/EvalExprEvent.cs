namespace NetTemplate.Debug;

using NetTemplate.Misc;

public class EvalExprEvent : InterpEvent
{
    // template pattern location
    private readonly Interval _sourceInterval;
    private readonly string expr = string.Empty;

    public EvalExprEvent(TemplateFrame frame, Interval outputInterval, Interval sourceInterval)
        : base(frame, outputInterval)
    {
        this._sourceInterval = sourceInterval;
        if (_sourceInterval != null)
            expr = frame.Template.impl.Template.Substring(_sourceInterval.Start, _sourceInterval.Length);
    }

    public Interval SourceInterval
    {
        get
        {
            return _sourceInterval;
        }
    }

    public string Expr
    {
        get
        {
            return expr;
        }
    }

    public override string ToString()
    {
        return string.Format("{0}{{self={1}, expr='{2}', source={3}, output={4}}}", GetType().Name, Template, Expr, SourceInterval, OutputInterval);
    }
}
