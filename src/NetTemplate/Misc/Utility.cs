namespace NetTemplate.Misc;

using StringBuilder = System.Text.StringBuilder;

public static class Utility
{
    public static string Strip(string s, int n)
    {
        return s.Substring(n, s.Length - 2 * n);
    }

    // strip newline from front but just one
    public static string TrimOneStartingNewline(string s)
    {
        if (s.StartsWith("\r\n"))
            s = s.Substring(2);
        else if (s.StartsWith("\n"))
            s = s.Substring(1);
        return s;
    }

    // strip newline from end but just one
    public static string TrimOneTrailingNewline(string s)
    {
        if (s.EndsWith("\r\n"))
            s = s.Substring(0, s.Length - 2);
        else if (s.EndsWith("\n"))
            s = s.Substring(0, s.Length - 1);
        return s;
    }

    public static string GetParent(string name)
    {
        if (name == null)
            return null;

        int lastSlash = name.LastIndexOf('/');
        if (lastSlash == 0)
            return "/";

        if (lastSlash > 0)
            return name.Substring(0, lastSlash);

        return string.Empty;
    }

    public static string GetPrefix(string name)
    {
        if (name == null)
            return "/";

        string parent = GetParent(name);
        string prefix = parent;
        if (!parent.EndsWith("/"))
            prefix += '/';

        return prefix;
    }

    public static string ReplaceEscapes(string s)
    {
        s = s.Replace("\n", "\\\\n");
        s = s.Replace("\r", "\\\\r");
        s = s.Replace("\t", "\\\\t");
        return s;
    }

    /** Replace >\> with >> in s. Replace \>> unless prefix of \>>> with >>.
     *  Do NOT replace if it's &lt;\\&gt;
     */
    public static string ReplaceEscapedRightAngle(string s)
    {
        StringBuilder buf = new StringBuilder();
        int i = 0;
        while (i < s.Length)
        {
            char c = s[i];
            if (c == '<' && s.Substring(i).StartsWith("<\\\\>"))
            {
                buf.Append("<\\\\>");
                i += "<\\\\>".Length;
                continue;
            }

            if (c == '>' && s.Substring(i).StartsWith(">\\>"))
            {
                buf.Append(">>");
                i += ">\\>".Length;
                continue;
            }

            if (c == '\\' && s.Substring(i).StartsWith("\\>>") &&
                !s.Substring(i).StartsWith("\\>>>"))
            {
                buf.Append(">>");
                i += "\\>>".Length;
                continue;
            }

            buf.Append(c);
            i++;
        }

        return buf.ToString();
    }

    /** Given index into string, compute the line and char position in line */
    public static Coordinate GetLineCharPosition(string s, int index)
    {
        int line = 1;
        int charPos = 0;
        int p = 0;
        while (p < index)
        {
            // don't care about s[index] itself; count before
            if (s[p] == '\n')
            {
                line++;
                charPos = 0;
            }
            else
            {
                charPos++;
            }

            p++;
        }

        return new Coordinate(line, charPos);
    }
}
