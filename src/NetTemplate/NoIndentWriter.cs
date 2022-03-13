namespace NetTemplate;

using TextWriter = System.IO.TextWriter;

/** Just pass through the text */
public class NoIndentWriter : AutoIndentWriter
{
    public NoIndentWriter(TextWriter writer)
        : base(writer)
    {
    }

    protected override int Indent()
    {
        return 0;
    }
}
