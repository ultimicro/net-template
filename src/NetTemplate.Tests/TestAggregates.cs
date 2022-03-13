namespace NetTemplate.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TestAggregates : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestApplyAnonymousTemplateToAggregateAttribute()
    {
        Template st = new Template("<items:{it|<it.id>: <it.lastName>, <it.firstName>\n}>");
        // also testing wacky spaces in aggregate spec
        st.AddMany("items.{ firstName ,lastName, id }", "Ter", "Parr", 99);
        st.AddMany("items.{firstName, lastName ,id}", "Tom", "Burns", 34);
        string expecting =
            "99: Parr, Ter" + newline +
            "34: Burns, Tom" + newline;
        Assert.AreEqual(expecting, st.Render());
    }

    public class Decl
    {
        private readonly string name;
        private readonly string type;

        public Decl(string name, string type)
        {
            this.name = name;
            this.type = type;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Type
        {
            get
            {
                return type;
            }
        }
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestComplicatedIndirectTemplateApplication()
    {
        string templates =
            "group Java;" + newline +
            "" + newline +
            "file(variables) ::= <<" +
            "<variables:{ v | <v.decl:(v.format)()>}; separator=\"\\n\">" + newline +
            ">>" + newline +
            "intdecl(decl) ::= \"int <decl.name> = 0;\"" + newline +
            "intarray(decl) ::= \"int[] <decl.name> = null;\"" + newline
            ;
        TemplateGroup group = new TemplateGroupString(templates);
        Template f = group.GetInstanceOf("file");
        f.AddMany("variables.{ decl,format }", new Decl("i", "int"), "intdecl");
        f.AddMany("variables.{decl ,  format}", new Decl("a", "int-array"), "intarray");
        //System.out.println("f='"+f+"'");
        string expecting = "int i = 0;" + newline +
                           "int[] a = null;";
        Assert.AreEqual(expecting, f.Render());
    }
}
