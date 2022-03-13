namespace NetTemplate.Compiler;

using System.Collections.Generic;
using NetTemplate.Misc;
using ArgumentException = System.ArgumentException;
using BitConverter = System.BitConverter;
using StringBuilder = System.Text.StringBuilder;

public class BytecodeDisassembler
{
    private readonly CompiledTemplate code;

    public BytecodeDisassembler(CompiledTemplate code)
    {
        this.code = code;
    }

    public virtual string GetInstructions()
    {
        StringBuilder buf = new StringBuilder();
        int ip = 0;
        while (ip < code.codeSize)
        {
            if (ip > 0)
                buf.Append(", ");
            int opcode = code.instrs[ip];
            Instruction I = Instruction.instructions[opcode];
            buf.Append(I.name);
            ip++;
            for (int opnd = 0; opnd < I.nopnds; opnd++)
            {
                buf.Append(' ');
                buf.Append(GetShort(code.instrs, ip));
                ip += Instruction.OperandSizeInBytes;
            }
        }
        return buf.ToString();
    }

    public virtual string Disassemble()
    {
        StringBuilder buf = new StringBuilder();
        int i = 0;
        while (i < code.codeSize)
        {
            i = DisassembleInstruction(buf, i);
            buf.AppendLine();
        }
        return buf.ToString();
    }

    public virtual int DisassembleInstruction(StringBuilder buf, int ip)
    {
        int opcode = code.instrs[ip];
        if (ip >= code.codeSize)
        {
            throw new ArgumentException("ip out of range: " + ip);
        }
        Instruction I = Instruction.instructions[opcode];
        if (I == null)
        {
            throw new ArgumentException("no such instruction " + opcode + " at address " + ip);
        }
        string instrName = I.name;
        buf.Append(string.Format("{0:0000}:\t{1,-14}", ip, instrName));
        ip++;
        if (I.nopnds == 0)
        {
            buf.Append("  ");
            return ip;
        }

        List<string> operands = new List<string>();
        for (int i = 0; i < I.nopnds; i++)
        {
            int opnd = GetShort(code.instrs, ip);
            ip += Instruction.OperandSizeInBytes;
            switch (I.type[i])
            {
                case OperandType.String:
                    operands.Add(ShowConstantPoolOperand(opnd));
                    break;

                case OperandType.Address:
                case OperandType.Int:
                    operands.Add(opnd.ToString());
                    break;

                default:
                    operands.Add(opnd.ToString());
                    break;
            }
        }

        for (int i = 0; i < operands.Count; i++)
        {
            string s = operands[i];
            if (i > 0)
                buf.Append(", ");

            buf.Append(s);
        }
        return ip;
    }

    private string ShowConstantPoolOperand(int poolIndex)
    {
        StringBuilder buf = new StringBuilder();
        buf.Append("#");
        buf.Append(poolIndex);
        string s = "<bad string index>";
        if (poolIndex < code.strings.Length)
        {
            if (code.strings[poolIndex] == null)
                s = "null";
            else
            {
                s = code.strings[poolIndex];
                if (code.strings[poolIndex] != null)
                {
                    s = Utility.ReplaceEscapes(s);
                    s = '"' + s + '"';
                }
            }
        }
        buf.Append(":");
        buf.Append(s);
        return buf.ToString();
    }

    internal static int GetShort(byte[] memory, int index)
    {
        return BitConverter.ToInt16(memory, index);
    }

    public virtual string GetStrings()
    {
        StringBuilder buf = new StringBuilder();
        int addr = 0;
        if (code.strings != null)
        {
            foreach (object o in code.strings)
            {
                if (o is string)
                {
                    string s = (string)o;
                    s = Utility.ReplaceEscapes(s);
                    buf.AppendLine(string.Format("{0:0000}: \"{1}\"", addr, s));
                }
                else
                {
                    buf.AppendLine(string.Format("{0:0000}: {1}", addr, o));
                }
                addr++;
            }
        }
        return buf.ToString();
    }

    public virtual string GetSourceMap()
    {
        StringBuilder buf = new StringBuilder();
        int addr = 0;
        foreach (Interval interval in code.sourceMap)
        {
            if (interval != null)
            {
                string chunk = code.Template.Substring(interval.Start, interval.Length);
                buf.AppendLine(string.Format("{0:0000}: {1}\t\"{2}\"", addr, interval, chunk));
            }
            addr++;
        }

        return buf.ToString();
    }
}
