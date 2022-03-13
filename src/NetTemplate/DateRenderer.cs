namespace NetTemplate;

using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;

/** A renderer for java.util.Date and Calendar objects. It understands a
 *  variety of format names as shown in formatToInt field.  By default
 *  it assumes "short" format.  A prefix of date: or time: shows only
 *  those components of the time object.
 */
public class DateRenderer : IAttributeRenderer
{
    public static readonly IDictionary<string, string> formatToInt =
        new Dictionary<string, string>() {
                {"short", "g"},
                {"medium", "g"},
                {"long", "f"},
                {"full", "F"},

                {"date:short", "d"},
                {"date:medium", "d"},
                {"date:long", "D"},
                {"date:full", "D"},

                {"time:short", "t"},
                {"time:medium", "t"},
                {"time:long", "T"},
                {"time:full", "T"},
        };

    public virtual string ToString(object o, string formatString, CultureInfo locale)
    {
        if (formatString == null)
            formatString = "short";

        DateTimeOffset d;
        if (o is DateTime)
            d = (DateTime)o;
        else if (o is DateTimeOffset)
            d = (DateTimeOffset)o;
        else
            throw new ArgumentException();

        string dateFormat;
        if (!formatToInt.TryGetValue(formatString, out dateFormat))
            return d.ToString(formatString, locale);

        return d.ToString(dateFormat, locale);
    }
}
