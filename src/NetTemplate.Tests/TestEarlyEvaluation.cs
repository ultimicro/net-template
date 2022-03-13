namespace NetTemplate.Tests;

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TestEarlyEvaluation : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestEarlyEvalInIfExpr()
    {
        string templates = "main(x) ::= << <if((x))>foo<else>bar<endif> >>";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");

        Template st = group.GetInstanceOf("main");

        string s = st.Render();
        Assert.AreEqual(" bar ", s);

        st.Add("x", "true");
        s = st.Render();
        Assert.AreEqual(" foo ", s);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestEarlyEvalOfSubtemplateInIfExpr()
    {
        string templates = "main(x) ::= << <if(({a<x>b}))>foo<else>bar<endif> >>";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");

        Template st = group.GetInstanceOf("main");

        string s = st.Render();
        Assert.AreEqual(" foo ", s);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestEarlyEvalOfMapInIfExpr()
    {
        string templates =
                "m ::= [\n" +
                "	\"parrt\": \"value\",\n" +
                "	default: \"other\"\n" +
                "]\n" +
                "main(x) ::= << p<x>t: <m.({p<x>t})>, <if(m.({p<x>t}))>if<else>else<endif> >>\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");

        Template st = group.GetInstanceOf("main");

        st.Add("x", null);
        string s = st.Render();
        Assert.AreEqual(" pt: other, if ", s);

        st.Add("x", "arr");
        s = st.Render();
        Assert.AreEqual(" parrt: value, if ", s);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestEarlyEvalOfMapInIfExprPassInHashMap()
    {
        string templates =
                "main(m,x) ::= << p<x>t: <m.({p<x>t})>, <if(m.({p<x>t}))>if<else>else<endif> >>\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroup group = new TemplateGroupFile(tmpdir + "/t.stg");

        Template st = group.GetInstanceOf("main");
        st.Add("m", new Dictionary<string, string> { { "parrt", "value" } });

        st.Add("x", null);
        string s = st.Render();
        Assert.AreEqual(" pt: , else ", s); // m[null] has no default value so else clause

        st.Add("x", "arr");
        s = st.Render();
        Assert.AreEqual(" parrt: value, if ", s);
    }
}
