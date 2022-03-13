namespace NetTemplate.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Path = System.IO.Path;

[TestClass]
public class TestNoNewlineTemplates : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestNoNewlineTemplate()
    {
        string template =
            "t(x) ::= <%" + newline +
            "[  <if(!x)>" +
            "<else>" +
            "<x>" + newline +
            "<endif>" +
            "" + newline +
            "" + newline +
            "]" + newline +
            "" + newline +
            "%>" + newline;
        TemplateGroup g = new TemplateGroupString(template);
        Template st = g.GetInstanceOf("t");
        st.Add("x", 99);
        string expected = "[  99]";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestWSNoNewlineTemplate()
    {
        string template =
            "t(x) ::= <%" + newline +
            "" + newline +
            "%>" + newline;
        TemplateGroup g = new TemplateGroupString(template);
        Template st = g.GetInstanceOf("t");
        st.Add("x", 99);
        string expected = "";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestEmptyNoNewlineTemplate()
    {
        string template =
            "t(x) ::= <%%>" + newline;
        TemplateGroup g = new TemplateGroupString(template);
        Template st = g.GetInstanceOf("t");
        st.Add("x", 99);
        string expected = "";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIgnoreIndent()
    {
        string template =
            "t(x) ::= <%" + newline +
            "	foo" + newline +
            "	<x>" + newline +
            "%>" + newline;
        TemplateGroup g = new TemplateGroupString(template);
        Template st = g.GetInstanceOf("t");
        st.Add("x", 99);
        string expected = "foo99";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIgnoreIndentInIF()
    {
        string template =
            "t(x) ::= <%" + newline +
            "	<if(x)>" + newline +
            "		foo" + newline +
            "	<endif>" + newline +
            "	<x>" + newline +
            "%>" + newline;
        TemplateGroup g = new TemplateGroupString(template);
        Template st = g.GetInstanceOf("t");
        st.Add("x", 99);
        string expected = "foo99";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestKeepWS()
    {
        string template =
            "t(x) ::= <%" + newline +
            "	<x> <x> hi" + newline +
            "%>" + newline;
        TemplateGroup g = new TemplateGroupString(template);
        Template st = g.GetInstanceOf("t");
        st.Add("x", 99);
        string expected = "99 99 hi";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestRegion()
    {
        string template =
            "t(x) ::= <%\n" +
            "<@r>\n" +
            "	Ignore\n" +
            "	newlines and indents\n" +
            "<x>\n\n\n" +
            "<@end>\n" +
            "%>\n";
        TemplateGroup g = new TemplateGroupString(template);
        Template st = g.GetInstanceOf("t");
        st.Add("x", 99);
        string expected = "Ignorenewlines and indents99";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDefineRegionInSubgroup()
    {
        string dir = tmpdir;
        string g1 = "a() ::= <<[<@r()>]>>\n";
        writeFile(dir, "g1.stg", g1);
        string g2 = "@a.r() ::= <%\n" +
        "	foo\n\n\n" +
        "%>\n";
        writeFile(dir, "g2.stg", g2);

        TemplateGroup group1 = new TemplateGroupFile(Path.Combine(dir, "g1.stg"));
        TemplateGroup group2 = new TemplateGroupFile(Path.Combine(dir, "g2.stg"));
        group2.ImportTemplates(group1); // define r in g2
        Template st = group2.GetInstanceOf("a");
        string expected = "[foo]";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }
}
