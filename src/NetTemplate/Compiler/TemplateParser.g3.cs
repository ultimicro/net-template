namespace NetTemplate.Compiler;

using Antlr.Runtime;
using NetTemplate.Misc;

partial class TemplateParser
{
    private readonly ErrorManager errMgr;
    private readonly IToken templateToken;

    public TemplateParser(ITokenStream input, ErrorManager errMgr, IToken templateToken)
        : this(input)
    {
        this.errMgr = errMgr;
        this.templateToken = templateToken;
    }

    protected override object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
    {
        throw new MismatchedTokenException(ttype, input);
    }
}
