namespace NetTemplate;

using CultureInfo = System.Globalization.CultureInfo;

/** Works with Byte, Short, Integer, Long, and BigInteger as well as
 *  Float, Double, and BigDecimal.  You pass in a Format string suitable
 *  for Formatter object:
 *
 *  http://java.sun.com/j2se/1.5.0/docs/api/java/util/Formatter.html
 *
 *  For example, "%10d" emits a number as a decimal int padding to 10 char.
 *  This can even do long to date conversions using the Format string.
 */
public class NumberRenderer : IAttributeRenderer
{
    public virtual string ToString(object o, string formatString, CultureInfo culture)
    {
        // o will be instanceof Number
        if (formatString == null)
            return o.ToString();

        return string.Format(culture, formatString, o);
    }
}
