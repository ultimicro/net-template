namespace NetTemplate.Misc;

public class TemplateModelAdaptor : IModelAdaptor
{
    public virtual object GetProperty(Interpreter interpreter, TemplateFrame frame, object o, object property, string propertyName)
    {
        Template template = (Template)o;
        return template.GetAttribute(propertyName);
    }
}
