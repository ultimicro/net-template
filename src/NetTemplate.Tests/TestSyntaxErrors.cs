namespace NetTemplate.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTemplate.Compiler;
using NetTemplate.Misc;
using NetTemplate.Tests.Extensions;
using Path = System.IO.Path;

[TestClass]
public class TestSyntaxErrors : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestEmptyExpr()
    {
        string template = " <> ";
        TemplateGroup group = new TemplateGroup();
        ErrorBuffer errors = new ErrorBuffer();
        group.Listener = errors;
        try
        {
            group.DefineTemplate("test", template);
        }
        catch (TemplateException)
        {
        }
        string result = errors.ToString();
        string expected = "test 1:0: this doesn't look like a template: \" <> \"" + newline;
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIt()
    {
        string templates =
            "main() ::= <<" + newline +
            "<@r>a<@end>" + newline +
            "<@r()>" + newline +
            ">>";
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        ErrorBuffer errors = new ErrorBuffer();
        group.Listener = errors;
        group.Load();
        Assert.AreEqual(0, errors.Errors.Count);

        Template template = group.GetInstanceOf("main");
        string expected =
            "a" + newline +
            "a";
        string result = template.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestEmptyExpr2()
    {
        string template = "hi <> ";
        TemplateGroup group = new TemplateGroup();
        ErrorBuffer errors = new ErrorBuffer();
        group.Listener = errors;
        try
        {
            group.DefineTemplate("test", template);
        }
        catch (TemplateException)
        {
        }
        string result = errors.ToString();
        string expected = "test 1:3: doesn't look like an expression" + newline;
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestUnterminatedExpr()
    {
        string template = "hi <t()$";
        TemplateGroup group = new TemplateGroup();
        ErrorBuffer errors = new ErrorBuffer();
        group.Listener = errors;
        try
        {
            group.DefineTemplate("test", template);
        }
        catch (TemplateException)
        {
        }
        string result = errors.ToString();
        string expected = "test 1:7: invalid character '$'" + newline +
            "test 1:7: invalid character '<EOF>'" + newline +
            "test 1:7: premature EOF" + newline;
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestWeirdChar()
    {
        string template = "   <*>";
        TemplateGroup group = new TemplateGroup();
        ErrorBuffer errors = new ErrorBuffer();
        group.Listener = errors;
        try
        {
            group.DefineTemplate("test", template);
        }
        catch (TemplateException)
        {
        }
        string result = errors.ToString();
        string expected = "test 1:4: invalid character '*'" + newline +
                          "test 1:0: this doesn't look like a template: \"   <*>\"" + newline;
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestWeirdChar2()
    {
        string template = "\n<\\\n";
        TemplateGroup group = new TemplateGroup();
        ErrorBuffer errors = new ErrorBuffer();
        group.Listener = errors;
        try
        {
            group.DefineTemplate("test", template);
        }
        catch (TemplateException)
        {
        }
        string result = errors.ToString();
        string expected = "test 1:2: invalid escaped char: '<EOF>'" + newline +
                          "test 1:2: expecting '>', found '<EOF>'" + newline;
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestValidButOutOfPlaceChar()
    {
        string templates =
            "foo() ::= <<hi <.> mom>>\n";
        writeFile(tmpdir, "t.stg", templates);

        ITemplateErrorListener errors = new ErrorBuffer();
        TemplateGroupFile group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 1:15: doesn't look like an expression" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestValidButOutOfPlaceCharOnDifferentLine()
    {
        string templates =
                "foo() ::= \"hi <\n" +
                ".> mom\"\n";
        writeFile(tmpdir, "t.stg", templates);

        ErrorBuffer errors = new ErrorBuffer();
        TemplateGroupFile group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "[t.stg 1:15: \\n in string, t.stg 1:14: doesn't look like an expression]";
        string result = errors.Errors.ToListString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestErrorInNestedTemplate()
    {
        string templates =
            "foo() ::= \"hi <name:{[<aaa.bb!>]}> mom\"\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ITemplateErrorListener errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 1:29: mismatched input '!' expecting RDELIM" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestEOFInExpr()
    {
        string templates =
            "foo() ::= \"hi <name:{x|[<aaa.bb>]}\"\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ITemplateErrorListener errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 1:34: premature EOF" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestEOFInExpr2()
    {
        string templates =
            "foo() ::= \"hi <name:{x|[<aaa.bb>]}\"\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ITemplateErrorListener errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 1:34: premature EOF" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestEOFInString()
    {
        string templates =
            "foo() ::= << <f(\"foo>>\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ITemplateErrorListener errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 1:20: EOF in string" + newline +
                          "t.stg 1:20: premature EOF" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestNonterminatedComment()
    {
        string templates = "foo() ::= << <!foo> >>";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ITemplateErrorListener errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected =
            "t.stg 1:20: Nonterminated comment starting at 1:1: '!>' missing" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestMissingRPAREN()
    {
        string templates =
            "foo() ::= \"hi <foo(>\"\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ITemplateErrorListener errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 1:19: '>' came as a complete surprise to me" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestRotPar()
    {
        string templates =
            "foo() ::= \"<a,b:t(),u()>\"\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ITemplateErrorListener errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 1:19: mismatched input ',' expecting RDELIM" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }
}
