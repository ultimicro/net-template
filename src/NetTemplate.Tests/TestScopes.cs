namespace NetTemplate.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTemplate.Misc;
using Path = System.IO.Path;

[TestClass]
public class TestScopes : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSeesEnclosingAttr()
    {
        string templates =
            "t(x,y) ::= \"<u()>\"\n" +
            "u() ::= \"<x><y>\"";
        ErrorBuffer errors = new ErrorBuffer();
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        Template st = group.GetInstanceOf("t");
        st.Add("x", "x");
        st.Add("y", "y");
        string result = st.Render();

        string expectedError = "";
        Assert.AreEqual(expectedError, errors.ToString());

        string expected = "xy";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestMissingArg()
    {
        string templates =
            "t() ::= \"<u()>\"\n" +
            "u(z) ::= \"\"";
        ErrorBuffer errors = new ErrorBuffer();
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        Template st = group.GetInstanceOf("t");
        string result = st.Render();

        string expectedError = "context [/t] 1:1 passed 0 arg(s) to template /u with 1 declared arg(s)" + newline;
        Assert.AreEqual(expectedError, errors.ToString());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestUnknownAttr()
    {
        string templates =
            "t() ::= \"<x>\"\n";
        ErrorBuffer errors = new ErrorBuffer();
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        Template st = group.GetInstanceOf("t");
        string result = st.Render();

        string expectedError = "context [/t] 1:1 attribute x isn't defined" + newline;
        Assert.AreEqual(expectedError, errors.ToString());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestArgWithSameNameAsEnclosing()
    {
        string templates =
            "t(x,y) ::= \"<u(x)>\"\n" +
            "u(y) ::= \"<x><y>\"";
        ErrorBuffer errors = new ErrorBuffer();
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        Template st = group.GetInstanceOf("t");
        st.Add("x", "x");
        st.Add("y", "y");
        string result = st.Render();

        string expectedError = "";
        Assert.AreEqual(expectedError, errors.ToString());

        string expected = "xx";
        Assert.AreEqual(expected, result);
        group.Listener = ErrorManager.DefaultErrorListener;
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestIndexAttrVisibleLocallyOnly()
    {
        string templates =
            "t(names) ::= \"<names:{n | <u(n)>}>\"\n" +
            "u(x) ::= \"<i>:<x>\"";
        ErrorBuffer errors = new ErrorBuffer();
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        Template st = group.GetInstanceOf("t");
        st.Add("names", "Ter");
        string result = st.Render();
        group.GetInstanceOf("u").impl.Dump();

        string expectedError = "t.stg 2:11: implicitly-defined attribute i not visible" + newline;
        Assert.AreEqual(expectedError, errors.ToString());

        string expected = ":Ter";
        Assert.AreEqual(expected, result);
        group.Listener = ErrorManager.DefaultErrorListener;
    }
}
