namespace NetTemplate.Compiler;

using Antlr.Runtime;
using NetTemplate.Misc;

partial class TemplateCompiler
{
    private class TemplateLexerNoNewlines : TemplateLexer
    {
        public TemplateLexerNoNewlines(ErrorManager errMgr, ICharStream input, IToken templateToken, char delimiterStartChar, char delimiterStopChar)
            : base(errMgr, input, templateToken, delimiterStartChar, delimiterStopChar)
        {
        }

        /// <summary>
        /// Throw out \n and leading whitespace tokens inside BIGSTRING_NO_NL.
        /// </summary>
        /// <returns></returns>
        public override IToken NextToken()
        {
            IToken t = base.NextToken();
            while (t.Type == TemplateLexer.NEWLINE || t.Type == TemplateLexer.INDENT)
            {
                t = base.NextToken();
            }

            return t;
        }
    }
}
