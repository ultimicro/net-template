namespace NetTemplate;

/// <summary>
/// An object that knows how to convert property references to appropriate
/// actions on a model object.  Some models, like JDBC, are interface based
/// (we aren't supposed to care about implementation classes). Some other
/// models don't follow getter method naming convention.  So, if we have
/// an object of type M with property method foo() (not getFoo()), we
/// register a model adaptor object, adap, that converts foo lookup to foo().
///
/// Given &lt;a.foo&gt;, we look up foo via the adaptor if "a instanceof(M)".
///
/// See unit tests.
/// </summary>
public interface IModelAdaptor
{
    /// <summary>
    /// Lookup property name in o and return its value.  It's a good
    /// idea to cache a Method or Field reflection object to make
    /// this fast after the first look up.
    ///
    /// property is normally a String but doesn't have to be. E.g.,
    /// if o is Map, property could be any key type.  If we need to convert
    /// to string, then it's done by Template and passed in here.
    /// </summary>
    object GetProperty(Interpreter interpreter, TemplateFrame frame, object obj, object property, string propertyName);
}
