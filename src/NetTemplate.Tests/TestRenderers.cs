namespace NetTemplate.Tests;

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CultureInfo = System.Globalization.CultureInfo;
using DateTime = System.DateTime;
using DateTimeOffset = System.DateTimeOffset;
using Path = System.IO.Path;

[TestClass]
public class TestRenderers : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestRendererForGroup()
    {
        string templates =
                "dateThing(created) ::= \"datetime: <created>\"\n";
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(DateTime), new DateRenderer());
        group.RegisterRenderer(typeof(DateTimeOffset), new DateRenderer());
        Template st = group.GetInstanceOf("dateThing");
        st.Add("created", new DateTime(2005, 7, 5));
        string expecting = "datetime: 07/05/2005 00:00";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestRendererWithFormat()
    {
        string templates =
                "dateThing(created) ::= << date: <created; format=\"yyyy.MM.dd\"> >>\n";
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        Template st = group.GetInstanceOf("dateThing");
        st.Add("created", new DateTime(2005, 7, 5));
        string expecting = " date: 2005.07.05 ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestRendererWithPredefinedFormat()
    {
        string templates =
                "dateThing(created) ::= << datetime: <created; format=\"short\"> >>\n";
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(DateTime), new DateRenderer());
        group.RegisterRenderer(typeof(DateTimeOffset), new DateRenderer());
        Template st = group.GetInstanceOf("dateThing");
        st.Add("created", new DateTime(2005, 7, 5));
        string expecting = " datetime: 07/05/2005 00:00 ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestRendererWithPredefinedFormat2()
    {
        string templates =
                "dateThing(created) ::= << datetime: <created; format=\"full\"> >>\n";
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(DateTime), new DateRenderer());
        group.RegisterRenderer(typeof(DateTimeOffset), new DateRenderer());
        Template st = group.GetInstanceOf("dateThing");
        st.Add("created", new DateTime(2005, 7, 5));
        string expecting = " datetime: Tuesday, 05 July 2005 00:00:00 ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [Ignore] // medium is not supported on .NET
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestRendererWithPredefinedFormat3()
    {
        string templates =
                "dateThing(created) ::= << date: <created; format=\"date:medium\"> >>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(DateTime), new DateRenderer());
        group.RegisterRenderer(typeof(DateTimeOffset), new DateRenderer());
        Template st = group.GetInstanceOf("dateThing");
        st.Add("created", new DateTime(2005, 7, 5));
        string expecting = " date: Jul 5, 2005 ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [Ignore] // medium is not supported on .NET
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestRendererWithPredefinedFormat4()
    {
        string templates =
                "dateThing(created) ::= << time: <created; format=\"time:medium\"> >>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(DateTime), new DateRenderer());
        group.RegisterRenderer(typeof(DateTimeOffset), new DateRenderer());
        Template st = group.GetInstanceOf("dateThing");
        st.Add("created", new DateTime(2005, 7, 5));
        string expecting = " time: 12:00:00 AM ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestStringRendererWithPrintfFormat()
    {
        string templates =
                "foo(x) ::= << <x; format=\"{0,6}\"> >>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(string), new StringRenderer());
        Template st = group.GetInstanceOf("foo");
        st.Add("x", "hi");
        string expecting = "     hi ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestRendererWithFormatAndList()
    {
        string template =
                "The names: <names; format=\"upper\">";
        var group = new TemplateGroup();
        group.RegisterRenderer(typeof(string), new StringRenderer());
        var st = new Template(group, template);
        st.Add("names", "ter");
        st.Add("names", "tom");
        st.Add("names", "sriram");
        string expecting = "The names: TERTOMSRIRAM";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestRendererWithFormatAndSeparator()
    {
        string template =
                "The names: <names; separator=\" and \", format=\"upper\">";
        var group = new TemplateGroup();
        group.RegisterRenderer(typeof(string), new StringRenderer());
        var st = new Template(group, template);
        st.Add("names", "ter");
        st.Add("names", "tom");
        st.Add("names", "sriram");
        string expecting = "The names: TER and TOM and SRIRAM";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestRendererWithFormatAndSeparatorAndNull()
    {
        string template =
                "The names: <names; separator=\" and \", null=\"n/a\", format=\"upper\">";
        var group = new TemplateGroup();
        group.RegisterRenderer(typeof(string), new StringRenderer());
        var st = new Template(group, template);
        var names = new List<string>
        {
            "ter",
            null,
            "sriram",
        };
        st.Add("names", names);
        string expecting = "The names: TER and N/A and SRIRAM";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestStringRendererWithFormat_cap()
    {
        string templates =
                "foo(x) ::= << <x; format=\"cap\"> >>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(string), new StringRenderer());
        Template st = group.GetInstanceOf("foo");
        st.Add("x", "hi");
        string expecting = " Hi ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestStringRendererWithTemplateInclude_cap()
    {
        // must toString the t() ref before applying format
        string templates =
                "foo(x) ::= << <(t()); format=\"cap\"> >>\n" +
                "t() ::= <<ack>>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(string), new StringRenderer());
        Template st = group.GetInstanceOf("foo");
        st.Add("x", "hi");
        string expecting = " Ack ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestStringRendererWithSubtemplateInclude_cap()
    {
        // must toString the t() ref before applying format
        string templates =
                "foo(x) ::= << <({ack}); format=\"cap\"> >>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(string), new StringRenderer());
        Template st = group.GetInstanceOf("foo");
        st.Add("x", "hi");
        string expecting = " Ack ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestStringRendererWithFormat_cap_emptyValue()
    {
        string templates =
                "foo(x) ::= << <x; format=\"cap\"> >>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(string), new StringRenderer());
        Template st = group.GetInstanceOf("foo");
        st.Add("x", string.Empty);
        string expecting = " "; // FIXME: why not two spaces?
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestStringRendererWithFormat_url_encode()
    {
        string templates =
                "foo(x) ::= << <x; format=\"url-encode\"> >>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(string), new StringRenderer());
        Template st = group.GetInstanceOf("foo");
        st.Add("x", "a b");
        string expecting = " a+b ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestStringRendererWithFormat_xml_encode()
    {
        string templates =
                "foo(x) ::= << <x; format=\"xml-encode\"> >>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(string), new StringRenderer());
        Template st = group.GetInstanceOf("foo");
        st.Add("x", "a<b> &\t\b");
        string expecting = " a&lt;b&gt; &amp;\t\b ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestStringRendererWithFormat_xml_encode_null()
    {
        string templates =
                "foo(x) ::= << <x; format=\"xml-encode\"> >>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(string), new StringRenderer());
        Template st = group.GetInstanceOf("foo");
        st.Add("x", null);
        string expecting = " ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestNumberRendererWithPrintfFormat()
    {
        string templates = "foo(x,y) ::= << <x; format=\"{0}\"> <y; format=\"{0:0.000}\"> >>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(int), new NumberRenderer());
        group.RegisterRenderer(typeof(double), new NumberRenderer());
        Template st = group.GetInstanceOf("foo");
        st.Add("x", -2100);
        st.Add("y", 3.14159);
        string expecting = " -2100 3.142 ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestInstanceofRenderer()
    {
        string templates =
                "numberThing(x,y,z) ::= \"numbers: <x>, <y>; <z>\"\n";
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(int), new NumberRenderer());
        group.RegisterRenderer(typeof(double), new NumberRenderer());
        Template st = group.GetInstanceOf("numberThing");
        st.Add("x", -2100);
        st.Add("y", 3.14159);
        st.Add("z", "hi");
        string expecting = "numbers: -2100, 3.14159; hi";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestLocaleWithNumberRenderer()
    {
        string templates = "foo(x,y) ::= << <x; format=\"{0:#,#}\"> <y; format=\"{0:0.000}\"> >>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.RegisterRenderer(typeof(int), new NumberRenderer());
        group.RegisterRenderer(typeof(double), new NumberRenderer());
        Template st = group.GetInstanceOf("foo");
        st.Add("x", -2100);
        st.Add("y", 3.14159);

        // Polish uses ' ' (ASCII 160) for ',' and ',' for '.'
        string expecting = " -2 100 3,142 "; // Ê
        string result = st.Render(new CultureInfo("pl"));
        Assert.AreEqual(expecting, result);
    }
}
