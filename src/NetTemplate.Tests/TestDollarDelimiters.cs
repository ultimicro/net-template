namespace NetTemplate.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Path = System.IO.Path;

[TestClass]
public class TestDollarDelimiters : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestAttr()
    {
        string template = "hi $name$!";
        Template st = new Template(template, '$', '$');
        st.Add("name", "Ter");
        string expected = "hi Ter!";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestParallelMap()
    {
        TemplateGroup group = new TemplateGroup('$', '$');
        group.DefineTemplate("test", "hi $names,phones:{n,p | $n$:$p$;}$", new string[] { "names", "phones" });
        Template st = group.GetInstanceOf("test");
        st.Add("names", "Ter");
        st.Add("names", "Tom");
        st.Add("names", "Sumana");
        st.Add("phones", "x5001");
        st.Add("phones", "x5002");
        st.Add("phones", "x5003");
        string expected =
            "hi Ter:x5001;Tom:x5002;Sumana:x5003;";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestRefToAnotherTemplateInSameGroup()
    {
        string dir = tmpdir;
        string a = "a() ::= << <$b()$> >>\n";
        string b = "b() ::= <<bar>>\n";
        writeFile(dir, "a.st", a);
        writeFile(dir, "b.st", b);
        TemplateGroup group = new TemplateGroupDirectory(dir, '$', '$');
        Template st = group.GetInstanceOf("a");
        string expected = " <bar> ";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDefaultArgument()
    {
        string templates =
                "method(name) ::= <<" + newline +
                "$stat(name)$" + newline +
                ">>" + newline +
                "stat(name,value=\"99\") ::= \"x=$value$; // $name$\"" + newline
                ;
        writeFile(tmpdir, "group.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "group.stg"), '$', '$');
        Template b = group.GetInstanceOf("method");
        b.Add("name", "foo");
        string expecting = "x=99; // foo";
        string result = b.Render();
        Assert.AreEqual(expecting, result);
    }

    /// <summary>
    /// This is part of a regression test for antlr/stringtemplate4#46.
    /// </summary>
    /// <seealso href="https://github.com/antlr/stringtemplate4/issues/46">STGroupString does not honor delimeter stanza in a string definition</seealso>
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDelimitersClause()
    {
        string templates =
                "delimiters \"$\", \"$\"" + newline +
                "method(name) ::= <<" + newline +
                "$stat(name)$" + newline +
                ">>" + newline +
                "stat(name,value=\"99\") ::= \"x=$value$; // $name$\"" + newline
                ;
        writeFile(tmpdir, "group.stg", templates);
        TemplateGroup group = new TemplateGroupFile(tmpdir + "/group.stg");
        Template b = group.GetInstanceOf("method");
        b.Add("name", "foo");
        string expecting = "x=99; // foo";
        string result = b.Render();
        Assert.AreEqual(expecting, result);
    }

    /// <summary>
    /// This is part of a regression test for antlr/stringtemplate4#46.
    /// </summary>
    /// <seealso href="https://github.com/antlr/stringtemplate4/issues/46">STGroupString does not honor delimeter stanza in a string definition</seealso>
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDelimitersClauseInGroupString()
    {
        string templates =
                "delimiters \"$\", \"$\"" + newline +
                "method(name) ::= <<" + newline +
                "$stat(name)$" + newline +
                ">>" + newline +
                "stat(name,value=\"99\") ::= \"x=$value$; // $name$\"" + newline
                ;
        TemplateGroup group = new TemplateGroupString(templates);
        Template b = group.GetInstanceOf("method");
        b.Add("name", "foo");
        string expecting = "x=99; // foo";
        string result = b.Render();
        Assert.AreEqual(expecting, result);
    }

    /// <summary>
    /// This is part of a regression test for antlr/stringtemplate4#66.
    /// </summary>
    /// <seealso href="https://github.com/antlr/stringtemplate4/issues/66">Changing delimiters doesn't work with STGroupFile</seealso>
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestImportTemplatePreservesDelimiters()
    {
        string groupFile =
            "group GenerateHtml;" + newline +
            "import \"html.st\"" + newline +
            "entry() ::= <<" + newline +
            "$html()$" + newline +
            ">>" + newline;
        string htmlFile =
            "html() ::= <<" + newline +
            "<table style=\"stuff\">" + newline +
            ">>" + newline;

        string dir = tmpdir;
        writeFile(dir, "GenerateHtml.stg", groupFile);
        writeFile(dir, "html.st", htmlFile);

        TemplateGroup group = new TemplateGroupFile(dir + "/GenerateHtml.stg", '$', '$');

        // test html template directly
        Template st = group.GetInstanceOf("html");
        Assert.IsNotNull(st);
        string expected = "<table style=\"stuff\">";
        string result = st.Render();
        Assert.AreEqual(expected, result);

        // test from entry template
        st = group.GetInstanceOf("entry");
        Assert.IsNotNull(st);
        expected = "<table style=\"stuff\">";
        result = st.Render();
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// This is part of a regression test for antlr/stringtemplate4#66.
    /// </summary>
    /// <seealso href="https://github.com/antlr/stringtemplate4/issues/66">Changing delimiters doesn't work with STGroupFile</seealso>
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestImportGroupPreservesDelimiters()
    {
        string groupFile =
            "group GenerateHtml;" + newline +
            "import \"HtmlTemplates.stg\"" + newline +
            "entry() ::= <<" + newline +
            "$html()$" + newline +
            ">>" + newline;
        string htmlFile =
            "html() ::= <<" + newline +
            "<table style=\"stuff\">" + newline +
            ">>" + newline;

        string dir = tmpdir;
        writeFile(dir, "GenerateHtml.stg", groupFile);
        writeFile(dir, "HtmlTemplates.stg", htmlFile);

        TemplateGroup group = new TemplateGroupFile(dir + "/GenerateHtml.stg", '$', '$');

        // test html template directly
        Template st = group.GetInstanceOf("html");
        Assert.IsNotNull(st);
        string expected = "<table style=\"stuff\">";
        string result = st.Render();
        Assert.AreEqual(expected, result);

        // test from entry template
        st = group.GetInstanceOf("entry");
        Assert.IsNotNull(st);
        expected = "<table style=\"stuff\">";
        result = st.Render();
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// This is part of a regression test for antlr/stringtemplate4#66.
    /// </summary>
    /// <seealso href="https://github.com/antlr/stringtemplate4/issues/66">Changing delimiters doesn't work with STGroupFile</seealso>
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDelimitersClauseOverridesConstructorDelimiters()
    {
        string groupFile =
            "group GenerateHtml;" + newline +
            "delimiters \"$\", \"$\"" + newline +
            "import \"html.st\"" + newline +
            "entry() ::= <<" + newline +
            "$html()$" + newline +
            ">>" + newline;
        string htmlFile =
            "html() ::= <<" + newline +
            "<table style=\"stuff\">" + newline +
            ">>" + newline;

        string dir = tmpdir;
        writeFile(dir, "GenerateHtml.stg", groupFile);
        writeFile(dir, "html.st", htmlFile);

        TemplateGroup group = new TemplateGroupFile(dir + "/GenerateHtml.stg", '<', '>');

        // test html template directly
        Template st = group.GetInstanceOf("html");
        Assert.IsNotNull(st);
        string expected = "<table style=\"stuff\">";
        string result = st.Render();
        Assert.AreEqual(expected, result);

        // test from entry template
        st = group.GetInstanceOf("entry");
        Assert.IsNotNull(st);
        expected = "<table style=\"stuff\">";
        result = st.Render();
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// This is part of a regression test for antlr/stringtemplate4#66.
    /// </summary>
    /// <seealso href="https://github.com/antlr/stringtemplate4/issues/66">Changing delimiters doesn't work with STGroupFile</seealso>
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDelimitersClauseOverridesInheritedDelimiters()
    {
        string groupFile =
            "group GenerateHtml;" + newline +
            "delimiters \"<\", \">\"" + newline +
            "import \"HtmlTemplates.stg\"" + newline +
            "entry() ::= <<" + newline +
            "<html()>" + newline +
            ">>" + newline;
        string htmlFile =
            "delimiters \"$\", \"$\"" + newline +
            "html() ::= <<" + newline +
            "<table style=\"stuff\">" + newline +
            ">>" + newline;

        string dir = tmpdir;
        writeFile(dir, "GenerateHtml.stg", groupFile);
        writeFile(dir, "HtmlTemplates.stg", htmlFile);

        TemplateGroup group = new TemplateGroupFile(dir + "/GenerateHtml.stg");

        // test html template directly
        Template st = group.GetInstanceOf("html");
        Assert.IsNotNull(st);
        string expected = "<table style=\"stuff\">";
        string result = st.Render();
        Assert.AreEqual(expected, result);

        // test from entry template
        st = group.GetInstanceOf("entry");
        Assert.IsNotNull(st);
        expected = "<table style=\"stuff\">";
        result = st.Render();
        Assert.AreEqual(expected, result);
    }
}
