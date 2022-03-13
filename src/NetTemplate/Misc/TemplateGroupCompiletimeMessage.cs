namespace NetTemplate.Misc;

using Exception = System.Exception;
using IToken = Antlr.Runtime.IToken;
using RecognitionException = Antlr.Runtime.RecognitionException;

public class TemplateGroupCompiletimeMessage : TemplateMessage
{
    /// <summary>
    /// token inside group file
    /// </summary>
    private readonly IToken _token;
    private readonly string _sourceName;

    public TemplateGroupCompiletimeMessage(ErrorType error, string sourceName, IToken token)
        : this(error, sourceName, token, null, null, null)
    {
    }

    public TemplateGroupCompiletimeMessage(ErrorType error, string sourceName, IToken token, Exception cause)
        : this(error, sourceName, token, cause, null, null)
    {
    }

    public TemplateGroupCompiletimeMessage(ErrorType error, string sourceName, IToken token, Exception cause, object arg)
        : this(error, sourceName, token, cause, arg, null)
    {
    }

    public TemplateGroupCompiletimeMessage(ErrorType error, string sourceName, IToken token, Exception cause, object arg, object arg2)
        : base(error, null, cause, arg, arg2)
    {
        this._token = token;
        this._sourceName = sourceName;
    }

    public IToken Token
    {
        get
        {
            return _token;
        }
    }

    public string SourceName
    {
        get
        {
            return _sourceName;
        }
    }

    public override string ToString()
    {
        RecognitionException re = (RecognitionException)Cause;
        int line = 0;
        int charPos = -1;
        if (_token != null)
        {
            line = _token.Line;
            charPos = _token.CharPositionInLine;
        }
        else if (re != null)
        {
            line = re.Line;
            charPos = re.CharPositionInLine;
        }

        string filepos = line + ":" + charPos;
        if (_sourceName != null)
        {
            return _sourceName + " " + filepos + ": " + string.Format(Error.Message, Arg, Arg2);
        }

        return filepos + ": " + string.Format(Error.Message, Arg, Arg2);
    }
}
