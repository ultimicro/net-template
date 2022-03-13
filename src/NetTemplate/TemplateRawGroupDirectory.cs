namespace NetTemplate;

using Antlr.Runtime;
using NetTemplate.Compiler;
using ArgumentException = System.ArgumentException;
using ArgumentNullException = System.ArgumentNullException;
using Console = System.Console;
using Directory = System.IO.Directory;
using Encoding = System.Text.Encoding;
using Exception = System.Exception;
using File = System.IO.File;
using IOException = System.IO.IOException;
using NotImplementedException = System.NotImplementedException;
using NotSupportedException = System.NotSupportedException;
using Path = System.IO.Path;
using StreamReader = System.IO.StreamReader;
using Uri = System.Uri;
using UriFormatException = System.UriFormatException;

// TODO: caching?

/** A dir of templates w/o headers like ST v3 had.  Still allows group files
 *  in dir though like TemplateGroupDirectory parent.
 */
public class TemplateRawGroupDirectory : TemplateGroupDirectory
{
    public TemplateRawGroupDirectory(string dirName)
        : base(dirName)
    {
    }

    public TemplateRawGroupDirectory(string dirName, char delimiterStartChar, char delimiterStopChar)
        : base(dirName, delimiterStartChar, delimiterStopChar)
    {
    }

    public TemplateRawGroupDirectory(string dirName, Encoding encoding)
        : base(dirName, encoding)
    {
    }

    public TemplateRawGroupDirectory(string dirName, Encoding encoding, char delimiterStartChar, char delimiterStopChar)
        : base(dirName, encoding, delimiterStartChar, delimiterStopChar)
    {
    }

    public TemplateRawGroupDirectory(Uri root, Encoding encoding, char delimiterStartChar, char delimiterStopChar)
        : base(root, encoding, delimiterStartChar, delimiterStopChar)
    {
    }

    public override CompiledTemplate LoadTemplateFile(string prefix, string unqualifiedFileName,
                                       ICharStream templateStream)
    {
        string template = templateStream.Substring(0, templateStream.Count);
        string templateName = Path.GetFileNameWithoutExtension(unqualifiedFileName);
        string fullyQualifiedTemplateName = prefix + templateName;
        CompiledTemplate impl = new TemplateCompiler(this).Compile(fullyQualifiedTemplateName, template);
        CommonToken nameT = new CommonToken(TemplateLexer.SEMI); // Seems like a hack, best I could come up with.
        nameT.InputStream = templateStream;
        RawDefineTemplate(fullyQualifiedTemplateName, impl, nameT);
        impl.DefineImplicitlyDefinedTemplates(this);
        return impl;
    }
}
