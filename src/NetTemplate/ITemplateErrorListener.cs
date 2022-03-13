namespace NetTemplate;

using NetTemplate.Misc;

/** How to handle messages */
public interface ITemplateErrorListener
{
    void CompiletimeError(TemplateMessage msg);
    void RuntimeError(TemplateMessage msg);
    void IOError(TemplateMessage msg);
    void InternalError(TemplateMessage msg);
}
