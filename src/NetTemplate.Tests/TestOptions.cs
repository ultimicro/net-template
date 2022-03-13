namespace NetTemplate.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTemplate.Misc;
using NetTemplate.Tests.Extensions;
using Console = System.Console;

[TestClass]
public class TestOptions : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSeparator()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("test", "hi <name; separator=\", \">!", new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        st.Add("name", "Ter");
        st.Add("name", "Tom");
        st.Add("name", "Sumana");
        string expected = "hi Ter, Tom, Sumana!";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSeparatorWithSpaces()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("test", "hi <name; separator= \", \">!", new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        Console.WriteLine(st.impl.Ast.ToStringTree());
        st.Add("name", "Ter");
        st.Add("name", "Tom");
        st.Add("name", "Sumana");
        string expected = "hi Ter, Tom, Sumana!";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestAttrSeparator()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("test", "hi <name; separator=sep>!", new string[] { "name", "sep" });
        Template st = group.GetInstanceOf("test");
        st.Add("sep", ", ");
        st.Add("name", "Ter");
        st.Add("name", "Tom");
        st.Add("name", "Sumana");
        string expected = "hi Ter, Tom, Sumana!";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIncludeSeparator()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("foo", "|");
        group.DefineTemplate("test", "hi <name; separator=foo()>!", new string[] { "name", "sep" });
        Template st = group.GetInstanceOf("test");
        st.Add("sep", ", ");
        st.Add("name", "Ter");
        st.Add("name", "Tom");
        st.Add("name", "Sumana");
        string expected = "hi Ter|Tom|Sumana!";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSubtemplateSeparator()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("test", "hi <name; separator={<sep> _}>!", new string[] { "name", "sep" });
        Template st = group.GetInstanceOf("test");
        st.Add("sep", ",");
        st.Add("name", "Ter");
        st.Add("name", "Tom");
        st.Add("name", "Sumana");
        string expected = "hi Ter, _Tom, _Sumana!";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSeparatorWithNullFirstValueAndNullOption()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("test", "hi <name; null=\"n/a\", separator=\", \">!", new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        st.Add("name", null);
        st.Add("name", "Tom");
        st.Add("name", "Sumana");
        string expected = "hi n/a, Tom, Sumana!";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSeparatorWithNull2ndValueAndNullOption()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("test", "hi <name; null=\"n/a\", separator=\", \">!", new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        st.impl.Dump();
        st.Add("name", "Ter");
        st.Add("name", null);
        st.Add("name", "Sumana");
        string expected = "hi Ter, n/a, Sumana!";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestNullValueAndNullOption()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("test", "<name; null=\"n/a\">", new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        st.Add("name", null);
        string expected = "n/a";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestListApplyWithNullValueAndNullOption()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("test", "<name:{n | <n>}; null=\"n/a\">", new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        st.Add("name", "Ter");
        st.Add("name", null);
        st.Add("name", "Sumana");
        string expected = "Tern/aSumana";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDoubleListApplyWithNullValueAndNullOption()
    {
        // first apply sends [Template, null, Template] to second apply, which puts [] around
        // the value.  This verifies that null not blank comes out of first apply
        // since we don't get [null].
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("test", "<name:{n | <n>}:{n | [<n>]}; null=\"n/a\">", new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        st.Add("name", "Ter");
        st.Add("name", null);
        st.Add("name", "Sumana");
        string expected = "[Ter]n/a[Sumana]";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestMissingValueAndNullOption()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("test", "<name; null=\"n/a\">", new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        string expected = "n/a";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestOptionDoesntApplyToNestedTemplate()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("foo", "<zippo>");
        group.DefineTemplate("test", "<foo(); null=\"n/a\">", new string[] { "zippo" });
        Template st = group.GetInstanceOf("test");
        st.Add("zippo", null);
        string expected = "";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIllegalOption()
    {
        ErrorBuffer errors = new ErrorBuffer();
        TemplateGroup group = new TemplateGroup();
        group.Listener = errors;
        group.DefineTemplate("test", "<name; bad=\"ugly\">", new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        st.Add("name", "Ter");
        string expected = "Ter";
        string result = st.Render();
        Assert.AreEqual(expected, result);
        expected = "[test 1:7: no such option: bad]";
        Assert.AreEqual(expected, errors.Errors.ToListString());
    }
}
