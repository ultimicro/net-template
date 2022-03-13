namespace NetTemplate.Debug;

#if !NETSTANDARD
    using System.Diagnostics;
#endif

/** An event that happens when building Template trees, adding attributes etc... */
public class ConstructionEvent
{
#if !NETSTANDARD
        private readonly StackTrace stack;
#endif

    public ConstructionEvent()
    {
#if !NETSTANDARD
            stack = new StackTrace(true);
#endif
    }

#if !NETSTANDARD
        public virtual string GetFileName()
        {
            return GetTemplateEntryPoint().GetFileName();
        }

        public virtual int GetLine()
        {
            return GetTemplateEntryPoint().GetFileLineNumber();
        }

        public virtual StackFrame GetTemplateEntryPoint()
        {
            StackFrame[] trace = stack.GetFrames();
            foreach (StackFrame e in trace)
            {
                string name = e.GetMethod().DeclaringType.FullName;
                if (!name.StartsWith("Antlr4.StringTemplate"))
                    return e;
            }

            return trace[0];
        }
#endif
}
