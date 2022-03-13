namespace NetTemplate.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTemplate.Misc;
using NetTemplate.Tests.Extensions;
using Path = System.IO.Path;

[TestClass]
public class TestGroupSyntaxErrors : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestMissingImportString()
    {
        string templates =
            "import\n" +
            "foo() ::= <<>>\n";
        writeFile(tmpdir, "t.stg", templates);

        ITemplateErrorListener errors = new ErrorBuffer();
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 2:0: mismatched input 'foo' expecting STRING" + newline +
            "t.stg 2:3: missing EndOfFile at '('" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestImportNotString()
    {
        string templates =
            "import Super.stg\n" +
            "foo() ::= <<>>\n";
        writeFile(tmpdir, "t.stg", templates);

        ITemplateErrorListener errors = new ErrorBuffer();
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 1:7: mismatched input 'Super' expecting STRING" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestMissingTemplate()
    {
        string templates =
            "foo() ::= \n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ITemplateErrorListener errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 2:0: missing template at '<EOF>'" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestUnclosedTemplate()
    {
        string templates =
            "foo() ::= {";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ITemplateErrorListener errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 1:11: missing final '}' in {...} anonymous template" + newline +
                          "t.stg 1:10: no viable alternative at input '{'" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestParen()
    {
        string templates =
            "foo( ::= << >>\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ITemplateErrorListener errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 1:5: no viable alternative at input '::='" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestNewlineInString()
    {
        string templates =
            "foo() ::= \"\nfoo\"\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ITemplateErrorListener errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 1:11: \\n in string" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestParen2()
    {
        string templates =
            "foo) ::= << >>\n" +
            "bar() ::= <<bar>>\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ITemplateErrorListener errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 1:0: garbled template definition starting at 'foo'" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestArg()
    {
        string templates =
            "foo(a,) ::= << >>\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ITemplateErrorListener errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "t.stg 1:6: missing ID at ')'" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestArg2()
    {
        string templates =
            "foo(a,,) ::= << >>\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ErrorBuffer errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected =
            "[t.stg 1:6: missing ID at ',', " +
            "t.stg 1:7: missing ID at ')']";
        string result = errors.Errors.ToListString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestArg3()
    {
        string templates =
            "foo(a b) ::= << >>\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ErrorBuffer errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected =
            "[t.stg 1:6: no viable alternative at input 'b']";
        string result = errors.Errors.ToListString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDefaultArgsOutOfOrder()
    {
        string templates =
            "foo(a={hi}, b) ::= << >>\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ErrorBuffer errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected =
            "[t.stg 1:13: Optional parameters must appear after all required parameters]";
        string result = errors.Errors.ToListString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestErrorWithinTemplate()
    {
        string templates =
            "foo(a) ::= \"<a b>\"\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ErrorBuffer errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "[t.stg 1:15: 'b' came as a complete surprise to me]";
        string result = errors.Errors.ToListString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestMap2()
    {
        string templates =
            "d ::= [\"k\":]\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ErrorBuffer errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "[t.stg 1:11: missing value for key at ']']";
        string result = errors.Errors.ToListString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestMap3()
    {
        string templates =
            "d ::= [\"k\":{dfkj}}]\n"; // extra }
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ErrorBuffer errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "[t.stg 1:17: invalid character '}']";
        string result = errors.Errors.ToListString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestUnterminatedString()
    {
        string templates =
            "f() ::= \""; // extra }
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group;
        ErrorBuffer errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        string expected = "[t.stg 1:9: unterminated string, t.stg 1:9: missing template at '<EOF>']";
        string result = errors.Errors.ToListString();
        Assert.AreEqual(expected, result);
    }
}
