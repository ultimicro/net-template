namespace NetTemplate.Compiler;

using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using NetTemplate.Misc;
using ArgumentNullException = System.ArgumentNullException;

/** A compiler for a single template. */
public partial class TemplateCompiler
{
    public static readonly string SubtemplatePrefix = "_sub";

    public const int InitialCodeSize = 15;

    public static readonly IDictionary<string, RenderOption> supportedOptions =
        new Dictionary<string, RenderOption>()
        {
                {"anchor",       RenderOption.Anchor},
                {"format",       RenderOption.Format},
                {"null",         RenderOption.Null},
                {"separator",    RenderOption.Separator},
                {"wrap",         RenderOption.Wrap},
        };

    public static readonly int NUM_OPTIONS = supportedOptions.Count;

    public static readonly IDictionary<string, string> defaultOptionValues =
        new Dictionary<string, string>()
        {
                {"anchor", "true"},
                {"wrap",   "\n"},
        };

    public static IDictionary<string, Bytecode> funcs =
        new Dictionary<string, Bytecode>()
        {
                {"first", Bytecode.INSTR_FIRST},
                {"last", Bytecode.INSTR_LAST},
                {"rest", Bytecode.INSTR_REST},
                {"trunc", Bytecode.INSTR_TRUNC},
                {"strip", Bytecode.INSTR_STRIP},
                {"trim", Bytecode.INSTR_TRIM},
                {"length", Bytecode.INSTR_LENGTH},
                {"strlen", Bytecode.INSTR_STRLEN},
                {"reverse", Bytecode.INSTR_REVERSE},
        };

    /** Name subtemplates _sub1, _sub2, ... */
    public static int subtemplateCount = 0;

    private readonly TemplateGroup _group;

    public TemplateCompiler(TemplateGroup group)
    {
        if (group == null)
            throw new ArgumentNullException("group");

        _group = group;
    }

    public TemplateGroup Group
    {
        get
        {
            return _group;
        }
    }

    public ErrorManager ErrorManager
    {
        get
        {
            return _group.ErrorManager;
        }
    }

    public char DelimiterStartChar
    {
        get
        {
            return _group.DelimiterStartChar;
        }
    }

    public char DelimiterStopChar
    {
        get
        {
            return _group.DelimiterStopChar;
        }
    }

    public virtual CompiledTemplate Compile(string template)
    {
        CompiledTemplate code = Compile(null, null, null, template, null);
        code.HasFormalArgs = false;
        return code;
    }

    /** Compile full template with unknown formal args. */
    public virtual CompiledTemplate Compile(string name, string template)
    {
        CompiledTemplate code = Compile(null, name, null, template, null);
        code.HasFormalArgs = false;
        return code;
    }

    /** Compile full template with respect to a list of formal args. */
    public virtual CompiledTemplate Compile(string srcName, string name, List<FormalArgument> args, string template, IToken templateToken)
    {
        ANTLRStringStream @is = new ANTLRStringStream(template, srcName);
        @is.name = srcName != null ? srcName : name;
        TemplateLexer lexer;
        if (templateToken != null && templateToken.Type == GroupParser.BIGSTRING_NO_NL)
        {
            lexer = new TemplateLexerNoNewlines(ErrorManager, @is, templateToken, DelimiterStartChar, DelimiterStopChar);
        }
        else
        {
            lexer = new TemplateLexer(ErrorManager, @is, templateToken, DelimiterStartChar, DelimiterStopChar);
        }

        CommonTokenStream tokens = new CommonTokenStream(lexer);
        TemplateParser p = new TemplateParser(tokens, ErrorManager, templateToken);
        IAstRuleReturnScope<CommonTree> r;
        try
        {
            r = p.templateAndEOF();
        }
        catch (RecognitionException re)
        {
            ReportMessageAndThrowTemplateException(tokens, templateToken, p, re);
            return null;
        }

        if (p.NumberOfSyntaxErrors > 0 || r.Tree == null)
        {
            CompiledTemplate impl = new CompiledTemplate();
            impl.DefineFormalArguments(args);
            return impl;
        }

        //System.out.println(((CommonTree)r.getTree()).toStringTree());
        CommonTreeNodeStream nodes = new CommonTreeNodeStream(r.Tree);
        nodes.TokenStream = tokens;
        CodeGenerator gen = new CodeGenerator(nodes, this, name, template, templateToken);

        CompiledTemplate impl2 = null;
        try
        {
            impl2 = gen.template(name, args);
            impl2.NativeGroup = Group;
            impl2.Template = template;
            impl2.Ast = r.Tree;
            impl2.Ast.SetUnknownTokenBoundaries();
            impl2.Tokens = tokens;
        }
        catch (RecognitionException re)
        {
            ErrorManager.InternalError(null, "bad tree structure", re);
        }

        return impl2;
    }

    public static CompiledTemplate DefineBlankRegion(CompiledTemplate outermostImpl, IToken nameToken)
    {
        if (outermostImpl == null)
            throw new ArgumentNullException("outermostImpl");
        if (nameToken == null)
            throw new ArgumentNullException("nameToken");

        string outermostTemplateName = outermostImpl.Name;
        string mangled = TemplateGroup.GetMangledRegionName(outermostTemplateName, nameToken.Text);
        CompiledTemplate blank = new CompiledTemplate();
        blank.IsRegion = true;
        blank.TemplateDefStartToken = nameToken;
        blank.RegionDefType = Template.RegionType.Implicit;
        blank.Name = mangled;
        outermostImpl.AddImplicitlyDefinedTemplate(blank);
        return blank;
    }

    public static string GetNewSubtemplateName()
    {
        subtemplateCount++;
        return SubtemplatePrefix + subtemplateCount;
    }

    protected virtual void ReportMessageAndThrowTemplateException(ITokenStream tokens, IToken templateToken, Parser parser, RecognitionException re)
    {
        if (re.Token.Type == TemplateLexer.EOF_TYPE)
        {
            string msg = "premature EOF";
            ErrorManager.CompiletimeError(ErrorType.SYNTAX_ERROR, templateToken, re.Token, msg);
        }
        else if (re is NoViableAltException)
        {
            string msg = "'" + re.Token.Text + "' came as a complete surprise to me";
            ErrorManager.CompiletimeError(ErrorType.SYNTAX_ERROR, templateToken, re.Token, msg);
        }
        else if (tokens.Index == 0)
        {
            // couldn't parse anything
            string msg = string.Format("this doesn't look like a template: \"{0}\"", tokens);
            ErrorManager.CompiletimeError(ErrorType.SYNTAX_ERROR, templateToken, re.Token, msg);
        }
        else if (tokens.LA(1) == TemplateLexer.LDELIM)
        {
            // couldn't parse expr
            string msg = "doesn't look like an expression";
            ErrorManager.CompiletimeError(ErrorType.SYNTAX_ERROR, templateToken, re.Token, msg);
        }
        else
        {
            string msg = parser.GetErrorMessage(re, parser.TokenNames);
            ErrorManager.CompiletimeError(ErrorType.SYNTAX_ERROR, templateToken, re.Token, msg);
        }

        // we have reported the error, so just blast out
        throw new TemplateException();
    }
}
