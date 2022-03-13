namespace NetTemplate.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Path = System.IO.Path;

[TestClass]
public class TestIndentation : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndentInFrontOfTwoExpr()
    {
        string templates =
                "list(a,b) ::= <<" +
                "  <a><b>" + newline +
                ">>" + newline;

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        Template t = group.GetInstanceOf("list");
        t.impl.Dump();
        t.Add("a", "Terence");
        t.Add("b", "Jim");
        string expecting =
                "  TerenceJim";
        Assert.AreEqual(expecting, t.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSimpleIndentOfAttributeList()
    {
        string templates =
                "list(names) ::= <<" +
                "  <names; separator=\"\\n\">" + newline +
                ">>" + newline;

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        Template t = group.GetInstanceOf("list");
        t.Add("names", "Terence");
        t.Add("names", "Jim");
        t.Add("names", "Sriram");
        string expecting =
                "  Terence" + newline +
                "  Jim" + newline +
                "  Sriram";
        Assert.AreEqual(expecting, t.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndentOfMultilineAttributes()
    {
        string templates =
                "list(names) ::= <<" +
                "  <names; separator=\"\n\">" + newline +
                ">>" + newline;
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        Template t = group.GetInstanceOf("list");
        t.Add("names", "Terence\nis\na\nmaniac");
        t.Add("names", "Jim");
        t.Add("names", "Sriram\nis\ncool");
        string expecting =
                "  Terence" + newline +
                "  is" + newline +
                "  a" + newline +
                "  maniac" + newline +
                "  Jim" + newline +
                "  Sriram" + newline +
                "  is" + newline +
                "  cool";
        Assert.AreEqual(expecting, t.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndentOfMultipleBlankLines()
    {
        string templates =
                "list(names) ::= <<" +
                "  <names>" + newline +
                ">>" + newline;
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        Template t = group.GetInstanceOf("list");
        t.Add("names", "Terence\n\nis a maniac");
        string expecting =
                "  Terence" + newline +
                "" + newline + // no indent on blank line
                "  is a maniac";
        Assert.AreEqual(expecting, t.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndentBetweenLeftJustifiedLiterals()
    {
        string templates =
                "list(names) ::= <<" +
                "Before:" + newline +
                "  <names; separator=\"\\n\">" + newline +
                "after" + newline +
                ">>" + newline;
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        Template t = group.GetInstanceOf("list");
        t.Add("names", "Terence");
        t.Add("names", "Jim");
        t.Add("names", "Sriram");
        string expecting =
                "Before:" + newline +
                "  Terence" + newline +
                "  Jim" + newline +
                "  Sriram" + newline +
                "after";
        Assert.AreEqual(expecting, t.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestNestedIndent()
    {
        string templates =
                "method(name,stats) ::= <<" +
                "void <name>() {" + newline +
                "\t<stats; separator=\"\\n\">" + newline +
                "}" + newline +
                ">>" + newline +
                "ifstat(expr,stats) ::= <<" + newline +
                "if (<expr>) {" + newline +
                "  <stats; separator=\"\\n\">" + newline +
                "}" +
                ">>" + newline +
                "assign(lhs,expr) ::= <<<lhs>=<expr>;>>" + newline
                ;
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        Template t = group.GetInstanceOf("method");
        t.Add("name", "foo");
        Template s1 = group.GetInstanceOf("assign");
        s1.Add("lhs", "x");
        s1.Add("expr", "0");
        Template s2 = group.GetInstanceOf("ifstat");
        s2.Add("expr", "x>0");
        Template s2a = group.GetInstanceOf("assign");
        s2a.Add("lhs", "y");
        s2a.Add("expr", "x+y");
        Template s2b = group.GetInstanceOf("assign");
        s2b.Add("lhs", "z");
        s2b.Add("expr", "4");
        s2.Add("stats", s2a);
        s2.Add("stats", s2b);
        t.Add("stats", s1);
        t.Add("stats", s2);
        string expecting =
                "void foo() {" + newline +
                "\tx=0;" + newline +
                "\tif (x>0) {" + newline +
                "\t  y=x+y;" + newline +
                "\t  z=4;" + newline +
                "\t}" + newline +
                "}";
        Assert.AreEqual(expecting, t.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndentedIFWithValueExpr()
    {
        Template t = new Template(
            "begin" + newline +
            "    <if(x)>foo<endif>" + newline +
            "end" + newline);
        t.Add("x", "x");
        string expecting =
            "begin" + newline +
            "    foo" + newline +
            "end" + newline;
        string result = t.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndentedIFWithElse()
    {
        Template t = new Template(
            "begin" + newline +
            "    <if(x)>foo<else>bar<endif>" + newline +
            "end" + newline);
        t.Add("x", "x");
        string expecting = "begin" + newline + "    foo" + newline + "end" + newline;
        string result = t.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndentedIFWithElse2()
    {
        Template t = new Template(
            "begin" + newline +
            "    <if(x)>foo<else>bar<endif>" + newline +
            "end" + newline);
        t.Add("x", false);
        string expecting = "begin" + newline + "    bar" + newline + "end" + newline;
        string result = t.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndentedIFWithNewlineBeforeText()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("t",
            "begin" + newline +
            "    <if(x)>\n" +
            "foo\n" +  // no indent; ignore IF indent
            "    <endif>" + newline +     // ignore indent on if-tags on line by themselves
            "end" + newline,
            new string[] { "x" });
        Template t = group.GetInstanceOf("t");
        t.Add("x", "x");
        string expecting = "begin" + newline + "foo" + newline + "end";
        string result = t.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndentedIFWithEndifNextLine()
    {
        TemplateGroup group = new TemplateGroup();
        group.DefineTemplate("t",
            "begin" + newline +
            "    <if(x)>foo\n" +      // use indent and keep newline
            "    <endif>" + newline +     // ignore indent on if-tags on line by themselves
            "end" + newline,
            new string[] { "x" });
        Template t = group.GetInstanceOf("t");
        t.Add("x", "x");
        string expecting = "begin" + newline + "    foo" + newline + "end";
        string result = t.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIFWithIndentOnMultipleLines()
    {
        Template t = new Template(
            "begin" + newline +
            "   <if(x)>" + newline +
            "   foo" + newline +
            "   <else>" + newline +
            "   bar" + newline +
            "   <endif>" + newline +
            "end" + newline);
        string expecting =
            "begin" + newline +
            "   bar" + newline +
            "end" + newline;
        string result = t.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIFWithIndentAndExprOnMultipleLines()
    {
        Template t = new Template(
            "begin" + newline +
            "   <if(x)>" + newline +
            "   <x>" + newline +
            "   <else>" + newline +
            "   <y>" + newline +
            "   <endif>" + newline +
            "end" + newline);
        t.Add("y", "y");
        string expecting =
            "begin" + newline +
            "   y" + newline +
            "end" + newline;
        string result = t.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIFWithIndentAndExprWithIndentOnMultipleLines()
    {
        Template t = new Template(
            "begin" + newline +
            "   <if(x)>" + newline +
            "     <x>" + newline +
            "   <else>" + newline +
            "     <y>" + newline +
            "   <endif>" + newline +
            "end" + newline);
        t.Add("y", "y");
        string expecting =
            "begin" + newline +
            "     y" + newline +
            "end" + newline;
        string result = t.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestNestedIFWithIndentOnMultipleLines()
    {
        Template t = new Template(
            "begin" + newline +
            "   <if(x)>" + newline +
            "      <if(y)>" + newline +
            "      foo" + newline +
            "      <endif>" + newline +
            "   <else>" + newline +
            "      <if(z)>" + newline +
            "      foo" + newline +
            "      <endif>" + newline +
            "   <endif>" + newline +
            "end" + newline);
        t.Add("x", "x");
        t.Add("y", "y");
        string expecting =
            "begin" + newline +
            "      foo" + newline +
            "end" + newline; // no indent
        string result = t.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIFInSubtemplate()
    {
        Template t = new Template(
            "<names:{n |" + newline +
            "   <if(x)>" + newline +
            "   <x>" + newline +
            "   <else>" + newline +
            "   <y>" + newline +
            "   <endif>" + newline +
            "}>" + newline);
        t.Add("names", "Ter");
        t.Add("y", "y");
        string expecting = "   y" + newline + newline;
        string result = t.Render();
        Assert.AreEqual(expecting, result);
    }
}
