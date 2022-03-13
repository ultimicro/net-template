namespace NetTemplate.Tests;

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErrorBuffer = NetTemplate.Misc.ErrorBuffer;

[TestClass]
public class TestIndirectionAndEarlyEval : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestEarlyEval()
    {
        string template = "<(name)>";
        Template st = new Template(template);
        st.Add("name", "Ter");
        string expected = "Ter";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndirectTemplateInclude()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("foo", "bar");
        string template = "<(name)()>";
        group.DefineTemplate("test", template, new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        st.Add("name", "foo");
        string expected = "bar";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndirectTemplateIncludeWithArgs()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("foo", "<x><y>", new string[] { "x", "y" });
        string template = "<(name)({1},{2})>";
        group.DefineTemplate("test", template, new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        st.Add("name", "foo");
        string expected = "12";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndirectCallWithPassThru()
    {
        // pass-through for dynamic template invocation is not supported by the
        // bytecode representation
        writeFile(tmpdir, "t.stg",
            "t1(x) ::= \"<x>\"\n" +
            "main(x=\"hello\",t=\"t1\") ::= <<\n" +
            "<(t)(...)>\n" +
            ">>");
        TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");
        ErrorBuffer errors = new ErrorBuffer();
        group.Listener = errors;
        Template st = group.GetInstanceOf("main");
        Assert.AreEqual("t.stg 2:34: mismatched input '...' expecting RPAREN" + newline, errors.ToString());
        Assert.IsNull(st);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndirectTemplateIncludeViaTemplate()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("foo", "bar");
        group.DefineTemplate("tname", "foo");
        string template = "<(tname())()>";
        group.DefineTemplate("test", template, new string[] { "name" });
        Template st = group.GetInstanceOf("test");
        string expected = "bar";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndirectProp()
    {
        string template = "<u.(propname)>: <u.name>";
        Template st = new Template(template);
        st.Add("u", new TestCoreBasics.User(1, "parrt"));
        st.Add("propname", "id");
        string expected = "1: parrt";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndirectMap()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("a", "[<x>]", new string[] { "x" });
        group.DefineTemplate("test", "hi <names:(templateName)()>!", new string[] { "names", "templateName" });
        Template st = group.GetInstanceOf("test");
        st.Add("names", "Ter");
        st.Add("names", "Tom");
        st.Add("names", "Sumana");
        st.Add("templateName", "a");
        string expected =
            "hi [Ter][Tom][Sumana]!";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestNonStringDictLookup()
    {
        string template = "<m.(intkey)>";
        Template st = new Template(template);
        IDictionary<int, string> m = new Dictionary<int, string>();
        m[36] = "foo";
        st.Add("m", m);
        st.Add("intkey", 36);
        string expected = "foo";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }
}
