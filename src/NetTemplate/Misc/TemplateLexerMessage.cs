namespace NetTemplate.Misc;

using NetTemplate.Compiler;
using Exception = System.Exception;
using IToken = Antlr.Runtime.IToken;
using RecognitionException = Antlr.Runtime.RecognitionException;

/** */
public class TemplateLexerMessage : TemplateMessage
{
    private readonly string _message;
    private readonly IToken _templateToken; // overall token pulled from group file
    private readonly string _sourceName;

    public TemplateLexerMessage(string sourceName, string message, IToken templateToken, Exception cause)
        : base(ErrorType.LEXER_ERROR, null, cause, null)
    {
        this._message = message;
        this._templateToken = templateToken;
        this._sourceName = sourceName;
    }

    public string Message
    {
        get
        {
            return _message;
        }
    }

    public IToken TemplateToken
    {
        get
        {
            return _templateToken;
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
        int line = re.Line;
        int charPos = re.CharPositionInLine;
        if (_templateToken != null)
        {
            int templateDelimiterSize = 1;
            if (_templateToken.Type == GroupParser.BIGSTRING)
            {
                templateDelimiterSize = 2;
            }
            line += _templateToken.Line - 1;
            charPos += _templateToken.CharPositionInLine + templateDelimiterSize;
        }

        string filepos = line + ":" + charPos;
        if (_sourceName != null)
        {
            return _sourceName + " " + filepos + ": " + string.Format(Error.Message, _message);
        }

        return filepos + ": " + string.Format(Error.Message, _message);
    }
}
