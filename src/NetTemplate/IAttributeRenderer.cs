namespace NetTemplate;

using CultureInfo = System.Globalization.CultureInfo;

/** This interface describes an object that knows how to Format or otherwise
 *  Render an object appropriately.  There is one renderer registered per
 *  group for a given Java type.
 *
 *  If the Format string passed to the renderer is not recognized then simply
 *  call ToString().
 *
 *  formatString can be null but locale will at least be Locale.getDefault()
 */
public interface IAttributeRenderer
{
    string ToString(object obj, string formatString, CultureInfo culture);
}
