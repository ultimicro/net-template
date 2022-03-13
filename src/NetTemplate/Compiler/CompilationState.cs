namespace NetTemplate.Compiler;

using Antlr.Runtime;
using Antlr.Runtime.Tree;
using NetTemplate.Misc;
using Array = System.Array;

/** temp data used during construction and functions that fill it / use it.
 *  Result is impl CompiledTemplate object.
 */
public class CompilationState
{
    /** The compiled code implementation to fill in. */
    internal CompiledTemplate impl = new CompiledTemplate();

    /** Track unique strings; copy into CompiledTemplate's String[] after compilation */
    internal StringTable stringtable = new StringTable();

    /** Track instruction location within code.instrs array; this is
     *  next address to Write to.  Byte-addressable memory.
     */
    internal int ip = 0;

    internal ITokenStream tokens;

    internal ErrorManager errMgr;

    public CompilationState(ErrorManager errMgr, string name, ITokenStream tokens)
    {
        this.errMgr = errMgr;
        this.tokens = tokens;
        impl.Name = name;
        impl.Prefix = Utility.GetPrefix(name);
    }

    public virtual int DefineString(string s)
    {
        return stringtable.Add(s);
    }

    public virtual void ReferenceAttribute(IToken templateToken, CommonTree id)
    {
        string name = id.Text;
        FormalArgument arg = impl.TryGetFormalArgument(name);
        if (arg != null)
        {
            int index = arg.Index;
            Emit1(id, Bytecode.INSTR_LOAD_LOCAL, index);
        }
        else
        {
            if (Interpreter.PredefinedAnonymousSubtemplateAttributes.Contains(name))
            {
                errMgr.CompiletimeError(ErrorType.REF_TO_IMPLICIT_ATTRIBUTE_OUT_OF_SCOPE, templateToken, id.Token);
                Emit(id, Bytecode.INSTR_NULL);
            }
            else
            {
                Emit1(id, Bytecode.INSTR_LOAD_ATTR, name);
            }
        }
    }

    public virtual void SetOption(CommonTree id)
    {
        RenderOption O = TemplateCompiler.supportedOptions[id.Text];
        Emit1(id, Bytecode.INSTR_STORE_OPTION, (int)O);
    }

    public virtual void Function(IToken templateToken, CommonTree id)
    {
        Bytecode funcBytecode;
        if (!TemplateCompiler.funcs.TryGetValue(id.Text, out funcBytecode))
        {
            errMgr.CompiletimeError(ErrorType.NO_SUCH_FUNCTION, templateToken, id.Token);
            Emit(id, Bytecode.INSTR_POP);
        }
        else
        {
            Emit(id, funcBytecode);
        }
    }

    public virtual void Emit(Bytecode opcode)
    {
        Emit(null, opcode);
    }

    public virtual void Emit(CommonTree opAST, Bytecode opcode)
    {
        EnsureCapacity(1);
        if (opAST != null)
        {
            int i = opAST.TokenStartIndex;
            int j = opAST.TokenStopIndex;
            int p = tokens.Get(i).StartIndex;
            int q = tokens.Get(j).StopIndex;
            if (!(p < 0 || q < 0))
                impl.sourceMap[ip] = Interval.FromBounds(p, q + 1);
        }
        impl.instrs[ip++] = (byte)opcode;
    }

    public virtual void Emit1(CommonTree opAST, Bytecode opcode, int arg)
    {
        Emit(opAST, opcode);
        EnsureCapacity(Instruction.OperandSizeInBytes);
        WriteShort(impl.instrs, ip, (short)arg);
        ip += Instruction.OperandSizeInBytes;
    }

    public virtual void Emit2(CommonTree opAST, Bytecode opcode, int arg, int arg2)
    {
        Emit(opAST, opcode);
        EnsureCapacity(Instruction.OperandSizeInBytes * 2);
        WriteShort(impl.instrs, ip, (short)arg);
        ip += Instruction.OperandSizeInBytes;
        WriteShort(impl.instrs, ip, (short)arg2);
        ip += Instruction.OperandSizeInBytes;
    }

    public virtual void Emit2(CommonTree opAST, Bytecode opcode, string s, int arg2)
    {
        int i = DefineString(s);
        Emit2(opAST, opcode, i, arg2);
    }

    public virtual void Emit1(CommonTree opAST, Bytecode opcode, string s)
    {
        int i = DefineString(s);
        Emit1(opAST, opcode, i);
    }

    public virtual void Insert(int addr, Bytecode opcode, string s)
    {
        //System.out.println("before insert of "+opcode+"("+s+"):"+ Arrays.toString(impl.instrs));
        EnsureCapacity(1 + Instruction.OperandSizeInBytes);
        int instrSize = 1 + Instruction.OperandSizeInBytes;
        // make room for opcode, opnd
        Array.Copy(impl.instrs, addr, impl.instrs, addr + instrSize, ip - addr);
        int save = ip;
        ip = addr;
        Emit1(null, opcode, s);
        ip = save + instrSize;
        //System.out.println("after  insert of "+opcode+"("+s+"):"+ Arrays.toString(impl.instrs));
        // adjust addresses for BR and BRF
        int a = addr + instrSize;
        while (a < ip)
        {
            Bytecode op = (Bytecode)impl.instrs[a];
            Instruction I = Instruction.instructions[(int)op];
            if (op == Bytecode.INSTR_BR || op == Bytecode.INSTR_BRF)
            {
                int opnd = BytecodeDisassembler.GetShort(impl.instrs, a + 1);
                WriteShort(impl.instrs, a + 1, (short)(opnd + instrSize));
            }
            a += I.nopnds * Instruction.OperandSizeInBytes + 1;
        }
        //System.out.println("after  insert of "+opcode+"("+s+"):"+ Arrays.toString(impl.instrs));
    }

    public virtual void Write(int addr, short value)
    {
        WriteShort(impl.instrs, addr, value);
    }

    protected virtual void EnsureCapacity(int n)
    {
        if ((ip + n) >= impl.instrs.Length)
        {
            // ensure room for full instruction
            Array.Resize(ref impl.instrs, impl.instrs.Length * 2);
            Array.Resize(ref impl.sourceMap, impl.sourceMap.Length * 2);
        }
    }

    public virtual void Indent(CommonTree indent)
    {
        Emit1(indent, Bytecode.INSTR_INDENT, indent.Text);
    }

    /** Write value at index into a byte array highest to lowest byte,
     *  left to right.
     */
    public static void WriteShort(byte[] memory, int index, short value)
    {
        memory[index + 0] = (byte)(value & 0xFF);
        memory[index + 1] = (byte)((value >> (8 * 1)) & 0xFF);
    }
}
