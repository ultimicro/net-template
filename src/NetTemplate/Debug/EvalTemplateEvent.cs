namespace NetTemplate.Debug;

using NetTemplate.Misc;

public class EvalTemplateEvent : InterpEvent
{
    public EvalTemplateEvent(TemplateFrame frame, Interval interval)
        : base(frame, interval)
    {
    }
}
