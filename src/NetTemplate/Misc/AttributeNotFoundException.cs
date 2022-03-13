namespace NetTemplate.Misc;

using NetTemplate.Compiler;

////** <name> where name is not found up the dynamic scoping chain. */
public class AttributeNotFoundException : TemplateException
{
    private readonly TemplateFrame _frame;
    private readonly string _attributeName;

    public AttributeNotFoundException(TemplateFrame frame, string attributeName)
    {
        _frame = frame;
        _attributeName = attributeName;
    }

    public override string Message
    {
        get
        {
            return "from template " + _frame.Template.Name + " no attribute " + _attributeName + " is visible";
        }
    }
}
