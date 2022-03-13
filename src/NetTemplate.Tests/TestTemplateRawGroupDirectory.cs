namespace NetTemplate.Tests;

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TestTemplateRawGroupDirectory : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSimpleGroup()
    {
        string dir = tmpdir;
        writeFile(dir, "a.st", "foo");
        TemplateGroup group = new TemplateRawGroupDirectory(dir, '$', '$');
        Template st = group.GetInstanceOf("a");
        string expected = "foo";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSimpleGroup2()
    {
        string dir = tmpdir;
        writeFile(dir, "a.st", "foo");
        writeFile(dir, "b.st", "$name$");
        TemplateGroup group = new TemplateRawGroupDirectory(dir, '$', '$');
        Template st = group.GetInstanceOf("a");
        string expected = "foo";
        string result = st.Render();
        Assert.AreEqual(expected, result);

        Template b = group.GetInstanceOf("b");
        b.Add("name", "Bob");
        Assert.AreEqual("Bob", b.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSimpleGroupAngleBrackets()
    {
        string dir = tmpdir;
        writeFile(dir, "a.st", "foo");
        writeFile(dir, "b.st", "<name>");
        TemplateGroup group = new TemplateRawGroupDirectory(dir);
        Template st = group.GetInstanceOf("a");
        string expected = "foo";
        string result = st.Render();
        Assert.AreEqual(expected, result);

        Template b = group.GetInstanceOf("b");
        b.Add("name", "Bob");
        Assert.AreEqual("Bob", b.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestAnonymousTemplateInRawTemplate()
    {
        string dir = tmpdir;
        writeFile(dir, "template.st", "$values:{foo|[$foo$]}$");
        TemplateGroup group = new TemplateRawGroupDirectory(dir, '$', '$');
        Template template = group.GetInstanceOf("template");
        List<string> values = new List<string>();
        values.Add("one");
        values.Add("two");
        values.Add("three");
        template.Add("values", values);
        Assert.AreEqual("[one][two][three]", template.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestMap()
    {
        string dir = tmpdir;
        writeFile(dir, "a.st", "$names:bold()$");
        writeFile(dir, "bold.st", "<b>$it$</b>");
        TemplateGroup group = new TemplateRawGroupDirectory(dir, '$', '$');
        Template st = group.GetInstanceOf("a");
        List<string> names = new List<string>();
        names.Add("parrt");
        names.Add("tombu");
        st.Add("names", names);
        //string asmResult = st.impl.GetInstructions();
        //Console.Out.WriteLine(asmResult);

        //st.Visualize();
        string expected = "<b>parrt</b><b>tombu</b>";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSuper()
    {
        string dir1 = tmpdir + "dir1011";
        string a = "dir1 a";
        string b = "dir1 b";
        writeFile(dir1, "a.st", a);
        writeFile(dir1, "b.st", b);
        string dir2 = tmpdir + "dir0220";
        a = "[<super.a()>]";
        writeFile(dir2, "a.st", a);

        TemplateGroup group1 = new TemplateRawGroupDirectory(dir1);
        TemplateGroup group2 = new TemplateRawGroupDirectory(dir2);
        group2.ImportTemplates(group1);
        Template st = group2.GetInstanceOf("a");
        string expected = "[dir1 a]";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// This is a regression test for antlr/stringtemplate4#70
    /// </summary>
    /// <seealso href="https://github.com/antlr/stringtemplate4/issues/70">Argument initialisation for sub-template in template with STRawGroupDir doesn't recognize valid parameters</seealso>
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestRawArgumentPassing()
    {
        string dir1 = tmpdir;
        string mainRawTemplate = "Hello $name$" + newline +
            "Then do the footer:" + newline +
            "$footerRaw(lastLine=veryLastLineRaw())$" + newline;
        string footerRawTemplate =
            "Simple footer. And now a last line:" + newline +
            "$lastLine$";
        string veryLastLineTemplate =
            "That's the last line.";
        writeFile(dir1, "mainRaw.st", mainRawTemplate);
        writeFile(dir1, "footerRaw.st", footerRawTemplate);
        writeFile(dir1, "veryLastLineRaw.st", veryLastLineTemplate);

        TemplateGroup group = new TemplateRawGroupDirectory(dir1, '$', '$');
        Template st = group.GetInstanceOf("mainRaw");
        Assert.IsNotNull(st);
        st.Add("name", "John");
        string result = st.Render();
        string expected =
            "Hello John" + newline +
            "Then do the footer:" + newline +
            "Simple footer. And now a last line:" + newline +
            "That's the last line." + newline;
        Assert.AreEqual(expected, result);
    }
}
