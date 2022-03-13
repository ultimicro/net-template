namespace NetTemplate.Compiler;

using Antlr.Runtime;
using NetTemplate.Misc;
using Path = System.IO.Path;

partial class GroupLexer
{
    public TemplateGroup group;

    public override void ReportError(RecognitionException e)
    {
        string msg = null;
        if (e is NoViableAltException)
        {
            msg = "invalid character '" + (char)input.LA(1) + "'";
        }
        else if (e is MismatchedTokenException && ((MismatchedTokenException)e).Expecting == '"')
        {
            msg = "unterminated string";
        }
        else
        {
            msg = GetErrorMessage(e, TokenNames);
        }

        group.ErrorManager.GroupSyntaxError(ErrorType.SYNTAX_ERROR, SourceName, e, msg);
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
}
