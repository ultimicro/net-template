namespace NetTemplate.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Path = System.IO.Path;

[TestClass]
public class TestLists : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestJustCat()
    {
        Template e = new Template(
                "<[names,phones]>"
            );
        e.Add("names", "Ter");
        e.Add("names", "Tom");
        e.Add("phones", "1");
        e.Add("phones", "2");
        string expecting = "TerTom12";
        Assert.AreEqual(expecting, e.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestListLiteralWithEmptyElements()
    {
        Template e = new Template(
            "<[\"Ter\",,\"Jesse\"]:{n | <i>:<n>}; separator=\", \", null={foo}>"
        );
        string expecting = "1:Ter, foo, 2:Jesse";
        Assert.AreEqual(expecting, e.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestListLiteralWithEmptyFirstElement()
    {
        Template e = new Template(
            "<[,\"Ter\",\"Jesse\"]:{n | <i>:<n>}; separator=\", \", null={foo}>"
        );
        string expecting = "foo, 1:Ter, 2:Jesse";
        Assert.AreEqual(expecting, e.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestLength()
    {
        Template e = new Template(
                "<length([names,phones])>"
            );
        e.Add("names", "Ter");
        e.Add("names", "Tom");
        e.Add("phones", "1");
        e.Add("phones", "2");
        string expecting = "4";
        Assert.AreEqual(expecting, e.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestCat2Attributes()
    {
        Template e = new Template(
                "<[names,phones]; separator=\", \">"
            );
        e.Add("names", "Ter");
        e.Add("names", "Tom");
        e.Add("phones", "1");
        e.Add("phones", "2");
        string expecting = "Ter, Tom, 1, 2";
        Assert.AreEqual(expecting, e.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestCat2AttributesWithApply()
    {
        Template e = new Template(
                "<[names,phones]:{a|<a>.}>"
            );
        e.Add("names", "Ter");
        e.Add("names", "Tom");
        e.Add("phones", "1");
        e.Add("phones", "2");
        string expecting = "Ter.Tom.1.2.";
        Assert.AreEqual(expecting, e.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestCat3Attributes()
    {
        Template e = new Template(
                "<[names,phones,salaries]; separator=\", \">"
            );
        e.Add("names", "Ter");
        e.Add("names", "Tom");
        e.Add("phones", "1");
        e.Add("phones", "2");
        e.Add("salaries", "big");
        e.Add("salaries", "huge");
        string expecting = "Ter, Tom, 1, 2, big, huge";
        Assert.AreEqual(expecting, e.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestCatWithTemplateApplicationAsElement()
    {
        Template e = new Template(
                "<[names:{n|<n>!},phones]; separator=\", \">"
            );
        e.Add("names", "Ter");
        e.Add("names", "Tom");
        e.Add("phones", "1");
        e.Add("phones", "2");
        string expecting = "Ter!, Tom!, 1, 2";
        Assert.AreEqual(expecting, e.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestCatWithIFAsElement()
    {
        Template e = new Template(
                "<[{<if(names)>doh<endif>},phones]; separator=\", \">"
            );
        e.Add("names", "Ter");
        e.Add("names", "Tom");
        e.Add("phones", "1");
        e.Add("phones", "2");
        string expecting = "doh, 1, 2";
        Assert.AreEqual(expecting, e.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestCatNullValues()
    {
        // [a, b] must behave like <a><b>; if a==b==null, blank output
        // unless null argument.
        Template e = new Template(
                "<[no,go]; null=\"foo\", separator=\", \">"
            );
        e.Add("phones", "1");
        e.Add("phones", "2");
        string expecting = "foo, foo";
        Assert.AreEqual(expecting, e.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestCatWithNullTemplateApplicationAsElement()
    {
        Template e = new Template(
                "<[names:{n|<n>!},\"foo\"]:{a|x}; separator=\", \">"
            );
        e.Add("phones", "1");
        e.Add("phones", "2");
        string expecting = "x";  // only one since template application gives nothing
        Assert.AreEqual(expecting, e.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestCatWithNestedTemplateApplicationAsElement()
    {
        Template e = new Template(
                "<[names, [\"foo\",\"bar\"]:{x | <x>!},phones]; separator=\", \">"
            );
        e.Add("names", "Ter");
        e.Add("names", "Tom");
        e.Add("phones", "1");
        e.Add("phones", "2");
        string expecting = "Ter, Tom, foo!, bar!, 1, 2";
        Assert.AreEqual(expecting, e.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestListAsTemplateArgument()
    {
        string templates =
                "test(names,phones) ::= \"<foo([names,phones])>\"" + newline +
                "foo(items) ::= \"<items:{a | *<a>*}>\"" + newline
                ;
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        Template e = group.GetInstanceOf("test");
        e.Add("names", "Ter");
        e.Add("names", "Tom");
        e.Add("phones", "1");
        e.Add("phones", "2");
        string expecting = "*Ter**Tom**1**2*";
        string result = e.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestListWithTwoEmptyListsCollapsesToEmptyList()
    {
        Template e = new Template(
            "<[[],[]]:{x | <x>!}; separator=\", \">"
        );
        e.Add("names", "Ter");
        e.Add("names", "Tom");
        string expecting = "";
        Assert.AreEqual(expecting, e.Render());
    }
}
