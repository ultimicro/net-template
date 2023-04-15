namespace NetTemplate.Tests;

using System;
using System.Globalization;
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
        var group = new TemplateGroup();
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
        var group = new TemplateGroup();
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
        var group = new TemplateGroup();
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
        var group = new TemplateGroup();
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
        var group = new TemplateGroup();
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
        var group = new TemplateGroup();
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
        var group = new TemplateGroup();
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
        var group = new TemplateGroup();
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
        var group = new TemplateGroup();
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
        var group = new TemplateGroup();
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
        var group = new TemplateGroup();
        group.DefineTemplate("test", "<name; null=\"n/a\">", new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        string expected = "n/a";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestCulture()
    {
        var group = new TemplateGroup();
        var renderer = new DateTimeRenderer();

        group.DefineTemplate("test", "<date; culture=\"th-TH\">", new string[] { "date" });
        group.RegisterRenderer(typeof(DateTime), renderer);

        var st = group.GetInstanceOf("test");
        st.Add("date", new DateTime(2023, 4, 15));

        Assert.AreEqual("15/4/2566 00:00:00", st.Render(CultureInfo.GetCultureInfo("en-US")));
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestInvariantCulture()
    {
        var group = new TemplateGroup();
        var renderer = new DateTimeRenderer();

        group.DefineTemplate("test", "<date; culture=\"\">", new string[] { "date" });
        group.RegisterRenderer(typeof(DateTime), renderer);

        var st = group.GetInstanceOf("test");
        st.Add("date", new DateTime(2023, 4, 15));

        Assert.AreEqual("04/15/2023 00:00:00", st.Render(CultureInfo.GetCultureInfo("th-TH")));
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDefaultCulture()
    {
        var group = new TemplateGroup();
        var renderer = new DateTimeRenderer();

        group.DefineTemplate("test", "<date>", new string[] { "date" });
        group.RegisterRenderer(typeof(DateTime), renderer);

        var st = group.GetInstanceOf("test");
        st.Add("date", new DateTime(2023, 4, 15));

        Assert.AreEqual("15/4/2566 00:00:00", st.Render(CultureInfo.GetCultureInfo("th-TH")));
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestOptionDoesntApplyToNestedTemplate()
    {
        var group = new TemplateGroup();
        group.DefineTemplate("foo", "<zippo>");
        group.DefineTemplate("test", "<foo(); null=\"n/a\">", new string[] { "zippo" });
        Template st = group.GetInstanceOf("test");
        st.Add("zippo", null);
        string expected = string.Empty;
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIllegalOption()
    {
        var errors = new ErrorBuffer();
        var group = new TemplateGroup
        {
            Listener = errors,
        };
        group.DefineTemplate("test", "<name; bad=\"ugly\">", new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        st.Add("name", "Ter");
        string expected = "Ter";
        string result = st.Render();
        Assert.AreEqual(expected, result);
        expected = "[test 1:7: no such option: bad]";
        Assert.AreEqual(expected, errors.Errors.ToListString());
    }

    private sealed class DateTimeRenderer : IAttributeRenderer
    {
        public string ToString(object obj, string formatString, CultureInfo culture)
        {
            return ((DateTime)obj).ToString(culture);
        }
    }
}
