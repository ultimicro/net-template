namespace NetTemplate.Debug;

using System.Collections.Generic;

/** Track all events that happen while evaluating this template */
public class DebugEvents
{
    /* Includes the EvalTemplateEvent for this template.  This
    *  is a subset of Interpreter.events field. The final
    *  EvalTemplateEvent is stored in 3 places:
    *
    *  	1. In enclosingInstance's childTemplateEvents
    *  	2. In this event list
    *  	3. In the overall event list
    *
    *  The root ST has the final EvalTemplateEvent in its list.
    *
    *  All events get added to the enclosingInstance's event list.
    */
    public readonly List<InterpEvent> Events = new List<InterpEvent>();

    /** All templates evaluated and embedded in this ST. Used
     *  for tree view in STViz.
     */
    public readonly List<EvalTemplateEvent> ChildEvalTemplateEvents = new List<EvalTemplateEvent>();

    public bool IsEarlyEval = false;
}
