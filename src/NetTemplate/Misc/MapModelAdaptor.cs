namespace NetTemplate.Misc;

using IDictionary = System.Collections.IDictionary;

public class MapModelAdaptor : IModelAdaptor
{
    public virtual object GetProperty(Interpreter interpreter, TemplateFrame frame, object o, object property, string propertyName)
    {
        object value;
        IDictionary map = (IDictionary)o;

        if (property == null)
            value = map[TemplateGroup.DefaultKey];
        else if (map.Contains(property))
            value = map[property];
        else if (map.Contains(propertyName))
            value = map[propertyName]; // if can't find the key, try ToString version
        else if (property.Equals("keys"))
            value = map.Keys;
        else if (property.Equals("values"))
            value = map.Values;
        else
            value = map[TemplateGroup.DefaultKey]; // not found, use default

        if (object.ReferenceEquals(value, TemplateGroup.DictionaryKey))
        {
            value = property;
        }

        return value;
    }
}
