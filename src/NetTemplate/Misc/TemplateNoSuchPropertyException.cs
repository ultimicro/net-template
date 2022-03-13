namespace NetTemplate.Misc;

using NetTemplate.Compiler;
using Exception = System.Exception;

/// <summary>
/// For the expression a.b, object a has no b property.
/// </summary>
public class TemplateNoSuchPropertyException : TemplateException
{
    private readonly object _object;
    private readonly string _propertyName;

    public TemplateNoSuchPropertyException()
    {
    }

    public TemplateNoSuchPropertyException(object obj, string propertyName)
    {
        this._object = obj;
        this._propertyName = propertyName;
    }

    public TemplateNoSuchPropertyException(object obj, string propertyName, Exception innerException)
        : base(null, innerException)
    {
        this._object = obj;
        this._propertyName = propertyName;
    }

    public string PropertyName
    {
        get
        {
            return _propertyName;
        }
    }
}
