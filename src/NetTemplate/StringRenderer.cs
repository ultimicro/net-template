namespace NetTemplate;

using CultureInfo = System.Globalization.CultureInfo;
using Encoding = System.Text.Encoding;
using HttpUtility = NetTemplate.Misc.HttpUtility;

/** This Render knows to perform a few operations on String objects:
 *  upper, lower, cap, url-encode, xml-encode.
 */
public class StringRenderer : IAttributeRenderer
{
    // trim(s) and strlen(s) built-in funcs; these are Format options
    public virtual string ToString(object o, string formatString, CultureInfo culture)
    {
        string s = (string)o;
        if (formatString == null)
            return s;

        if (formatString.Equals("upper"))
            return culture.TextInfo.ToUpper(s);

        if (formatString.Equals("lower"))
            return culture.TextInfo.ToLower(s);

        if (formatString.Equals("cap"))
            return s.Length > 0 ? culture.TextInfo.ToUpper(s[0]) + s.Substring(1) : s;

        if (formatString.Equals("url-encode"))
            return HttpUtility.UrlEncode(s, Encoding.UTF8);

        if (formatString.Equals("xml-encode"))
        {
            return s.Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

        return string.Format(culture, formatString, s);
    }
}
