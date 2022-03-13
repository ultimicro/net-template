namespace NetTemplate.Compiler;

using Antlr.Runtime;
using Antlr.Runtime.Tree;
using NetTemplate.Misc;

partial class CodeGenerator
{
    /// <summary>Name of overall template</summary>
    private readonly string outermostTemplateName;
    /// <summary>Overall template token</summary>
    private readonly IToken templateToken;
    /// <summary>Overall template text</summary>
    private readonly string _template;
    private readonly TemplateCompiler _compiler;
    private CompiledTemplate outermostImpl;

    public CodeGenerator(ITreeNodeStream input, TemplateCompiler compiler, string name, string template, IToken templateToken)
        : this(input, new RecognizerSharedState())
    {
        this._compiler = compiler;
        this.outermostTemplateName = name;
        this._template = template;
        this.templateToken = templateToken;
    }

    public ErrorManager errMgr
    {
        get
        {
            return _compiler.ErrorManager;
        }
    }

    public TemplateGroup Group
    {
        get
        {
            return _compiler.Group;
        }
    }

    public CompilationState CompilationState
    {
        get
        {
            if (template_stack == null || template_stack.Count == 0)
                return null;

            return template_stack.Peek().state;
        }
    }

    // convience funcs to hide offensive sending of emit messages to
    // CompilationState temp data object.

    public void emit1(CommonTree opAST, Bytecode opcode, int arg)
    {
        CompilationState.Emit1(opAST, opcode, arg);
    }

    public void emit1(CommonTree opAST, Bytecode opcode, string arg)
    {
        CompilationState.Emit1(opAST, opcode, arg);
    }

    public void emit2(CommonTree opAST, Bytecode opcode, int arg, int arg2)
    {
        CompilationState.Emit2(opAST, opcode, arg, arg2);
    }

    public void emit2(CommonTree opAST, Bytecode opcode, string s, int arg2)
    {
        CompilationState.Emit2(opAST, opcode, s, arg2);
    }

    public void emit(CommonTree opAST, Bytecode opcode)
    {
        CompilationState.Emit(opAST, opcode);
    }

    private void Indent(CommonTree indent)
    {
        CompilationState.Indent(indent);
    }

    private void Dedent()
    {
        CompilationState.Emit(Bytecode.INSTR_DEDENT);
    }

    public void insert(int addr, Bytecode opcode, string s)
    {
        CompilationState.Insert(addr, opcode, s);
    }

    public void setOption(CommonTree id)
    {
        CompilationState.SetOption(id);
    }

    public void write(int addr, short value)
    {
        CompilationState.Write(addr, value);
    }

    public int address()
    {
        return CompilationState.ip;
    }

    public void func(CommonTree id)
    {
        CompilationState.Function(templateToken, id);
    }

    public void refAttr(CommonTree id)
    {
        CompilationState.ReferenceAttribute(templateToken, id);
    }

    public int defineString(string s)
    {
        return CompilationState.DefineString(s);
    }
}
