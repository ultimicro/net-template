namespace NetTemplate.Compiler;

using Antlr.Runtime;
using NetTemplate.Misc;
using Path = System.IO.Path;

partial class GroupParser
{
    private TemplateGroup _group;

    public TemplateGroup Group
    {
        get
        {
            return _group;
        }

        internal set
        {
            _group = value;
        }
    }

    public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
    {
        string msg = GetErrorMessage(e, tokenNames);
        _group.ErrorManager.GroupSyntaxError(ErrorType.SYNTAX_ERROR, SourceName, e, msg);
    }

    public override string SourceName
    {
        get
        {
            string fullFileName = base.SourceName;
            // strip to simple name
            return Path.GetFileName(fullFileName);
        }
    }

    public virtual void Error(string msg)
    {
        NoViableAltException e = new NoViableAltException(string.Empty, 0, 0, input);
        _group.ErrorManager.GroupSyntaxError(ErrorType.SYNTAX_ERROR, SourceName, e, msg);
        Recover(input, null);
    }
}
