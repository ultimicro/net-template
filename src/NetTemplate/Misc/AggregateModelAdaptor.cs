namespace NetTemplate.Misc;

using ArgumentException = System.ArgumentException;

/** Deal with structs created via ST.add("structname.{prop1, prop2}", ...); */
public class AggregateModelAdaptor : MapModelAdaptor
{
    public override object GetProperty(Interpreter interpreter, TemplateFrame frame, object o, object property, string propertyName)
    {
        Aggregate aggregate = o as Aggregate;
        if (aggregate == null)
            throw new ArgumentException();

        return base.GetProperty(interpreter, frame, aggregate.Properties, property, propertyName);
    }
}
