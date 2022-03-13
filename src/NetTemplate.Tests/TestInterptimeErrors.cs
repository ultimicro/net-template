namespace NetTemplate.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTemplate.Misc;
using Environment = System.Environment;
using Path = System.IO.Path;

[TestClass]
public class TestInterptimeErrors : BaseTest
{
    public class UserHiddenName
    {
        protected string name;
        public UserHiddenName(string name)
        {
            this.name = name;
        }
        protected string getName()
        {
            return name;
        }
    }

    public class UserHiddenNameField
    {
        protected string name;
        public UserHiddenNameField(string name)
        {
            this.name = name;
        }
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestMissingEmbeddedTemplate()
    {
        ErrorBuffer errors = new ErrorBuffer();

        string templates =
            "t() ::= \"<foo()>\"" + Environment.NewLine;

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        Template st = group.GetInstanceOf("t");
        st.Render();
        string expected = "context [/t] 1:1 no such template: /foo" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestMissingSuperTemplate()
    {
        ErrorBuffer errors = new ErrorBuffer();

        string templates =
            "t() ::= \"<super.t()>\"" + Environment.NewLine;

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        string templates2 =
            "u() ::= \"blech\"" + Environment.NewLine;

        writeFile(tmpdir, "t2.stg", templates2);
        TemplateGroup group2 = new TemplateGroupFile(Path.Combine(tmpdir, "t2.stg"));
        group.ImportTemplates(group2);
        Template st = group.GetInstanceOf("t");
        st.Render();
        string expected = "context [/t] 1:1 no such template: super.t" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestNoPropertyNotError()
    {
        ErrorBuffer errors = new ErrorBuffer();

        string templates =
            "t(u) ::= \"<u.x>\"" + Environment.NewLine;

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        Template st = group.GetInstanceOf("t");
        st.Add("u", new User(32, "parrt"));
        st.Render();
        string expected = "";
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestHiddenPropertyNotError()
    {
        ErrorBuffer errors = new ErrorBuffer();

        string templates =
            "t(u) ::= \"<u.name>\"" + Environment.NewLine;

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        Template st = group.GetInstanceOf("t");
        st.Add("u", new UserHiddenName("parrt"));
        st.Render();
        string expected = "";
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestHiddenFieldNotError()
    {
        ErrorBuffer errors = new ErrorBuffer();

        string templates =
            "t(u) ::= \"<u.name>\"" + Environment.NewLine;

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        Template st = group.GetInstanceOf("t");
        st.Add("u", new UserHiddenNameField("parrt"));
        st.Render();
        string expected = "";
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSoleArg()
    {
        ErrorBuffer errors = new ErrorBuffer();

        string templates =
            "t() ::= \"<u({9})>\"\n" +
            "u(x,y) ::= \"<x>\"\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        Template st = group.GetInstanceOf("t");
        st.Render();
        string expected = "context [/t] 1:1 passed 1 arg(s) to template /u with 2 declared arg(s)" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSoleArgUsingApplySyntax()
    {
        ErrorBuffer errors = new ErrorBuffer();

        string templates =
            "t() ::= \"<{9}:u()>\"\n" +
            "u(x,y) ::= \"<x>\"\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        Template st = group.GetInstanceOf("t");
        string expected = "9";
        string result = st.Render();
        Assert.AreEqual(expected, result);

        expected = "context [/t] 1:5 passed 1 arg(s) to template /u with 2 declared arg(s)" + newline;
        result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestUndefinedAttr()
    {
        ErrorBuffer errors = new ErrorBuffer();

        string templates =
            "t() ::= \"<u()>\"\n" +
            "u() ::= \"<x>\"\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        Template st = group.GetInstanceOf("t");
        st.Render();
        string expected = "context [/t /u] 1:1 attribute x isn't defined" + newline;
        string result = errors.ToString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestParallelAttributeIterationWithMissingArgs()
    {
        ErrorBuffer errors = new ErrorBuffer();
        TemplateGroup group = new TemplateGroup();
        group.Listener = errors;
        Template e = new Template(group,
                "<names,phones,salaries:{n,p | <n>@<p>}; separator=\", \">"
            );
        e.Add("names", "Ter");
        e.Add("names", "Tom");
        e.Add("phones", "1");
        e.Add("phones", "2");
        e.Add("salaries", "big");
        e.Render();
        string errorExpecting =
            "1:23: anonymous template has 2 arg(s) but mapped across 3 value(s)" + newline +
            "context [anonymous] 1:23 passed 3 arg(s) to template /_sub1 with 2 declared arg(s)" + newline +
            "context [anonymous] 1:1 iterating through 3 values in zip map but template has 2 declared arguments" + newline;
        Assert.AreEqual(errorExpecting, errors.ToString());
        string expecting = "Ter@1, Tom@2";
        Assert.AreEqual(expecting, e.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestStringTypeMismatch()
    {
        ErrorBuffer errors = new ErrorBuffer();
        TemplateGroup group = new TemplateGroup();
        group.Listener = errors;
        Template e = new Template(group, "<trim(s)>");
        e.Add("s", 34);
        e.Render(); // generate the error
        string errorExpecting = "context [anonymous] 1:1 function trim expects a string not System.Int32" + newline;
        Assert.AreEqual(errorExpecting, errors.ToString());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestStringTypeMismatch2()
    {
        ErrorBuffer errors = new ErrorBuffer();
        TemplateGroup group = new TemplateGroup();
        group.Listener = errors;
        Template e = new Template(group, "<strlen(s)>");
        e.Add("s", 34);
        e.Render(); // generate the error
        string errorExpecting = "context [anonymous] 1:1 function strlen expects a string not System.Int32" + newline;
        Assert.AreEqual(errorExpecting, errors.ToString());
    }
}
