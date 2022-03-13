namespace NetTemplate;

public interface ITypeProxyFactory
{
    object CreateProxy(TemplateFrame frame, object obj);
}
