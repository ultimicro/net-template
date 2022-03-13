namespace NetTemplate;

using Antlr.Runtime;
using NetTemplate.Compiler;
using NetTemplate.Misc;
using Exception = System.Exception;

/// <summary>
/// A group derived from a string not a file or dir.
/// </summary>
public class TemplateGroupString : TemplateGroup
{
    private string sourceName;
    private string text;
    private bool alreadyLoaded = false;

    public TemplateGroupString(string text)
        : this("[string]", text, '<', '>')
    {
    }

    public TemplateGroupString(string sourceName, string text)
        : this(sourceName, text, '<', '>')
    {
    }

    public TemplateGroupString(string sourceName, string text, char delimiterStartChar, char delimiterStopChar)
        : base(delimiterStartChar, delimiterStopChar)
    {
        this.sourceName = sourceName;
        this.text = text;
    }

    public override string FileName
    {
        get
        {
            return sourceName;
        }
    }

    public override bool IsDefined(string name)
    {
        if (!alreadyLoaded)
            Load();

        return base.IsDefined(name);
    }

    public override void Load()
    {
        if (alreadyLoaded)
            return;

        alreadyLoaded = true;
        GroupParser parser;
        try
        {
            ANTLRStringStream fs = new ANTLRStringStream(text);
            fs.name = sourceName;
            GroupLexer lexer = new GroupLexer(fs);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            parser = new GroupParser(tokens);
            // no prefix since this group file is the entire group, nothing lives
            // beneath it.
            parser.group(this, "/");
        }
        catch (Exception e)
        {
            ErrorManager.IOError(null, ErrorType.CANT_LOAD_GROUP_FILE, e, FileName);
        }
    }

    protected override CompiledTemplate Load(string name)
    {
        if (!alreadyLoaded)
            Load();
        return RawGetTemplate(name);
    }
}
