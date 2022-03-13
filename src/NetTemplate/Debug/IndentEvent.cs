namespace NetTemplate.Debug;

using NetTemplate.Misc;

public class IndentEvent : EvalExprEvent
{
    public IndentEvent(TemplateFrame frame, Interval outputInterval, Interval sourceInterval)
        : base(frame, outputInterval, sourceInterval)
    {
    }
}
