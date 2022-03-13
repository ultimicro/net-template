namespace NetTemplate.Tests.Extensions;

using CultureInfo = System.Globalization.CultureInfo;
using IList = System.Collections.IList;

internal static class ListExtensions
{
    public static string ToListString(this IList list)
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("listTemplate", "[<list:{x|<x>}; separator=\", \">]", new string[] { "list" });
        group.RegisterRenderer(typeof(IList), new CollectionRenderer());
        Template st = group.GetInstanceOf("listTemplate");
        st.Add("list", list);
        return st.Render();
    }

    private class CollectionRenderer : IAttributeRenderer
    {
        public string ToString(object o, string formatString, CultureInfo culture)
        {
            return ((IList)o).ToListString();
        }
    }
}
