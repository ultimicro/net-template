﻿namespace NetTemplate.Tests;

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTemplate.Misc;
using NetTemplate.Tests.Extensions;
using File = System.IO.File;
using Path = System.IO.Path;

[TestClass]
public class TestDictionaries : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDict()
    {
        string templates =
                "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("var");
        st.Add("type", "int");
        st.Add("name", "x");
        string expecting = "int x = 0;";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestEmptyDictionary()
    {
        string templates =
            "d ::= []\n";
        writeFile(tmpdir, "t.stg", templates);

        TemplateGroupFile group = null;
        ErrorBuffer errors = new ErrorBuffer();
        group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        group.Listener = errors;
        group.Load(); // force load
        Assert.AreEqual(0, errors.Errors.Count);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictValuesAreTemplates()
    {
        string templates =
                "typeInit ::= [\"int\":{0<w>}, \"float\":{0.0<w>}] " + newline +
                "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("var");
        st.impl.Dump();
        st.Add("w", "L");
        st.Add("type", "int");
        st.Add("name", "x");
        string expecting = "int x = 0L;";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictKeyLookupViaTemplate()
    {
        // Make sure we try rendering stuff to string if not found as regular object
        string templates =
                "typeInit ::= [\"int\":{0<w>}, \"float\":{0.0<w>}] " + newline +
                "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("var");
        st.Add("w", "L");
        st.Add("type", new Template("int"));
        st.Add("name", "x");
        string expecting = "int x = 0L;";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictKeyLookupAsNonToStringableObject()
    {
        // Make sure we try rendering stuff to string if not found as regular object
        string templates =
                "foo(m,k) ::= \"<m.(k)>\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("foo");
        IDictionary<HashableUser, string> m = new Dictionary<HashableUser, string>();
        m[new HashableUser(99, "parrt")] = "first";
        m[new HashableUser(172036, "tombu")] = "second";
        m[new HashableUser(391, "sriram")] = "third";
        st.Add("m", m);
        st.Add("k", new HashableUser(172036, "tombu"));
        string expecting = "second";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictMissingDefaultValueIsEmpty()
    {
        string templates =
                "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("var");
        st.Add("w", "L");
        st.Add("type", "double"); // double not in typeInit map
        st.Add("name", "x");
        string expecting = "double x = ;";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictMissingDefaultValueIsEmptyForNullKey()
    {
        string templates =
                "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                "var(type,w,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("var");
        st.Add("w", "L");
        st.Add("type", null); // double not in typeInit map
        st.Add("name", "x");
        string expecting = " x = ;";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictHiddenByFormalArg()
    {
        string templates =
                "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                "var(typeInit,type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("var");
        st.Add("type", "int");
        st.Add("name", "x");
        string expecting = "int x = ;";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictEmptyValueAndAngleBracketStrings()
    {
        string templates =
                "typeInit ::= [\"int\":\"0\", \"float\":, \"double\":<<0.0L>>] " + newline +
                "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("var");
        st.Add("type", "float");
        st.Add("name", "x");
        string expecting = "float x = ;";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictDefaultValue()
    {
        string templates =
                "typeInit ::= [\"int\":\"0\", default:\"null\"] " + newline +
                "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("var");
        st.Add("type", "UserRecord");
        st.Add("name", "x");
        string expecting = "UserRecord x = null;";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictNullKeyGetsDefaultValue()
    {
        string templates =
                "typeInit ::= [\"int\":\"0\", default:\"null\"] " + newline +
                "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("var");
        // missing or set to null: st.Add("type", null);
        st.Add("name", "x");
        string expecting = " x = null;";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictEmptyDefaultValue()
    {
        string templates =
                "typeInit ::= [\"int\":\"0\", default:] " + newline +
                "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        ErrorBuffer errors = new ErrorBuffer();
        TemplateGroupFile group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        group.Listener = errors;
        group.Load();
        string expected = "[test.stg 1:33: missing value for key at ']']";
        string result = errors.Errors.ToListString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictDefaultValueIsKey()
    {
        string templates =
                "typeInit ::= [\"int\":\"0\", default:key] " + newline +
                "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("var");
        st.Add("type", "UserRecord");
        st.Add("name", "x");
        string expecting = "UserRecord x = UserRecord;";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictWithoutIteration()
    {
        string templates =
            "t2(adr,line2={<adr.zip> <adr.city>}) ::= <<" + newline +
            "<adr.firstname> <adr.lastname>" + newline +
            "<line2>" + newline +
            ">>";

        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("t2");
        st.Add("adr", new Dictionary<string, string>()
                {
                    {"firstname","Terence"},
                    {"lastname","Parr"},
                    {"zip","99999"},
                    {"city","San Francisco"},
                });
        string expecting =
            "Terence Parr" + newline +
            "99999 San Francisco";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictWithoutIteration2()
    {
        string templates =
            "t2(adr,line2={<adr.zip> <adr.city>}) ::= <<" + newline +
            "<adr.firstname> <adr.lastname>" + newline +
            "<line2>" + newline +
            ">>";

        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("t2");
        st.Add("adr", new Dictionary<string, string>()
                {
                    {"firstname","Terence"},
                    {"lastname","Parr"},
                    {"zip","99999"},
                    {"city","San Francisco"},
                });
        st.Add("line2", new Template("<adr.city>, <adr.zip>"));
        string expecting =
            "Terence Parr" + newline +
            "San Francisco, 99999";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictWithoutIteration3()
    {
        string templates =
            "t2(adr,line2={<adr.zip> <adr.city>}) ::= <<" + newline +
            "<adr.firstname> <adr.lastname>" + newline +
            "<line2>" + newline +
            ">>" + newline +
            "t3(adr) ::= <<" + newline +
            "<t2(adr=adr,line2={<adr.city>, <adr.zip>})>" + newline +
            ">>" + newline;

        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("t3");
        st.Add("adr", new Dictionary<string, string>()
                {
                    {"firstname","Terence"},
                    {"lastname","Parr"},
                    {"zip","99999"},
                    {"city","San Francisco"},
                });
        string expecting =
            "Terence Parr" + newline +
            "San Francisco, 99999";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    /**
     * Test that a map can have only the default entry.
     */
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictDefaultStringAsKey()
    {
        string templates =
                "typeInit ::= [\"default\":\"foo\"] " + newline +
                "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("var");
        st.Add("type", "default");
        st.Add("name", "x");
        string expecting = "default x = foo;";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    /**
     * Test that a map can return a <b>string</b> with the word: default.
     */
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictDefaultIsDefaultString()
    {
        string templates =
                "map ::= [default: \"default\"] " + newline +
                "t() ::= << <map.(\"1\")> >>" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("t");
        string expecting = " default ";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictViaEnclosingTemplates()
    {
        string templates =
                "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                "intermediate(type,name) ::= \"<var(type,name)>\"" + newline +
                "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template st = group.GetInstanceOf("intermediate");
        st.Add("type", "int");
        st.Add("name", "x");
        string expecting = "int x = 0;";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictViaEnclosingTemplates2()
    {
        string templates =
                "typeInit ::= [\"int\":\"0\", \"float\":\"0.0\"] " + newline +
                "intermediate(stuff) ::= \"<stuff>\"" + newline +
                "var(type,name) ::= \"<type> <name> = <typeInit.(type)>;\"" + newline
                ;
        writeFile(tmpdir, "test.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "test.stg"));
        Template interm = group.GetInstanceOf("intermediate");
        Template var = group.GetInstanceOf("var");
        var.Add("type", "int");
        var.Add("name", "x");
        interm.Add("stuff", var);
        string expecting = "int x = 0;";
        string result = interm.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestAccessDictionaryFromAnonymousTemplate()
    {
        string dir = tmpdir;
        string g =
            "a() ::= <<[<[\"foo\",\"a\"]:{x|<if(values.(x))><x><endif>}>]>>\n" +
            "values ::= [\n" +
            "    \"a\":false,\n" +
            "    default:true\n" +
            "]\n";
        writeFile(dir, "g.stg", g);

        TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));
        Template st = group.GetInstanceOf("a");
        string expected = "[foo]";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestAccessDictionaryFromAnonymousTemplateInRegion()
    {
        string dir = tmpdir;
        string g =
            "a() ::= <<[<@r()>]>>\n" +
            "@a.r() ::= <<\n" +
            "<[\"foo\",\"a\"]:{x|<if(values.(x))><x><endif>}>\n" +
            ">>\n" +
            "values ::= [\n" +
            "    \"a\":false,\n" +
            "    default:true\n" +
            "]\n";
        writeFile(dir, "g.stg", g);

        TemplateGroup group = new TemplateGroupFile(Path.Combine(dir, "g.stg"));
        Template st = group.GetInstanceOf("a");
        string expected = "[foo]";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestStringsInDictionary()
    {
        string templates =
            "auxMap ::= [\n" +
            "   \"E\": \"electric <field>\",\n" +
            "   \"I\": \"in <field> between\",\n" +
            "   \"F\": \"<field> force\",\n" +
            "   default: \"<field>\"\n" +
            "]\n" +
            "\n" +
            "makeTmpl(type, field) ::= <<\n" +
            "<auxMap.(type)>\n" +
            ">>\n" +
            "\n" +
            "top() ::= <<\n" +
            "  <makeTmpl(\"E\", \"foo\")>\n" +
            "  <makeTmpl(\"F\", \"foo\")>\n" +
            "  <makeTmpl(\"I\", \"foo\")>\n" +
            ">>\n";
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(tmpdir + Path.DirectorySeparatorChar + "t.stg");
        Template st = group.GetInstanceOf("top");
        Assert.IsNotNull(st);
        string expecting =
            "  electric <field>" + newline +
            "  <field> force" + newline +
            "  in <field> between";
        Assert.AreEqual(expecting, st.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestTemplatesInDictionary()
    {
        string templates =
            "auxMap ::= [\n" +
            "   \"E\": {electric <field>},\n" +
            "   \"I\": {in <field> between},\n" +
            "   \"F\": {<field> force},\n" +
            "   default: {<field>}\n" +
            "]\n" +
            "\n" +
            "makeTmpl(type, field) ::= <<\n" +
            "<auxMap.(type)>\n" +
            ">>\n" +
            "\n" +
            "top() ::= <<\n" +
            "  <makeTmpl(\"E\", \"foo\")>\n" +
            "  <makeTmpl(\"F\", \"foo\")>\n" +
            "  <makeTmpl(\"I\", \"foo\")>\n" +
            ">>\n";
        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(tmpdir + Path.DirectorySeparatorChar + "t.stg");
        Template st = group.GetInstanceOf("top");
        Assert.IsNotNull(st);
        string expecting =
            "  electric foo" + newline +
            "  foo force" + newline +
            "  in foo between";
        Assert.AreEqual(expecting, st.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictionaryBehaviorTrue()
    {
        string templates =
            "d ::= [\n" +
            "	\"x\" : true,\n" +
            "	default : false,\n" +
            "]\n" +
            "\n" +
            "t() ::= <<\n" +
            "<d.(\"x\")><if(d.(\"x\"))>+<else>-<endif>\n" +
            ">>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(tmpdir + Path.DirectorySeparatorChar + "t.stg");
        Template st = group.GetInstanceOf("t");
        string expected = "true+";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictionaryBehaviorFalse()
    {
        string templates =
            "d ::= [\n" +
            "	\"x\" : false,\n" +
            "	default : false,\n" +
            "]\n" +
            "\n" +
            "t() ::= <<\n" +
            "<d.(\"x\")><if(d.(\"x\"))>+<else>-<endif>\n" +
            ">>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(tmpdir + Path.DirectorySeparatorChar + "t.stg");
        Template st = group.GetInstanceOf("t");
        string expected = "false-";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictionaryBehaviorEmptyTemplate()
    {
        string templates =
            "d ::= [\n" +
            "	\"x\" : {},\n" +
            "	default : false,\n" +
            "]\n" +
            "\n" +
            "t() ::= <<\n" +
            "<d.(\"x\")><if(d.(\"x\"))>+<else>-<endif>\n" +
            ">>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(tmpdir + Path.DirectorySeparatorChar + "t.stg");
        Template st = group.GetInstanceOf("t");
        string expected = "+";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictionaryBehaviorEmptyList()
    {
        string templates =
            "d ::= [\n" +
            "	\"x\" : [],\n" +
            "	default : false,\n" +
            "]\n" +
            "\n" +
            "t() ::= <<\n" +
            "<d.(\"x\")><if(d.(\"x\"))>+<else>-<endif>\n" +
            ">>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(tmpdir + Path.DirectorySeparatorChar + "t.stg");
        Template st = group.GetInstanceOf("t");
        string expected = "-";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// This is a regression test for antlr/stringtemplate4#114. Before the fix the following test would return
    /// %hi%.
    /// </summary>
    /// <seealso href="https://github.com/antlr/stringtemplate4/issues/114">dictionary value using &lt;% %&gt; is broken</seealso>
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictionaryBehaviorNoNewlineTemplate()
    {
        string templates =
            "d ::= [\n" +
            "	\"x\" : <%hi%>\n" +
            "]\n" +
            "\n" +
            "t() ::= <<\n" +
            "<d.x>\n" +
            ">>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(tmpdir + Path.DirectorySeparatorChar + "t.stg");
        Template st = group.GetInstanceOf("t");
        string expected = "hi";
        string result = st.Render();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictionarySpecialValues()
    {
        string templates = @"
t(id) ::= <<
<identifier.(id)>
>>

identifier ::= [
    ""keyword"" : ""@keyword"",
    default : key
]
";

        writeFile(tmpdir, "t.stg", templates);
        var group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));

        // try with mapped values
        var template = group.GetInstanceOf("t").Add("id", "keyword");
        Assert.AreEqual("@keyword", template.Render());

        // try with non-mapped values
        template = group.GetInstanceOf("t").Add("id", "nonkeyword");
        Assert.AreEqual("nonkeyword", template.Render());

        // try with non-mapped values that might break (Substring here guarantees unique instances)
        template = group.GetInstanceOf("t").Add("id", "_default".Substring(1));
        Assert.AreEqual("default", template.Render());

        template = group.GetInstanceOf("t").Add("id", "_keys".Substring(1));
        Assert.AreEqual("keyworddefault", template.Render());

        template = group.GetInstanceOf("t").Add("id", "_values".Substring(1));
        Assert.AreEqual("@keywordkey", template.Render());
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestDictionarySpecialValuesOverride()
    {
        string templates = @"
t(id) ::= <<
<identifier.(id)>
>>

identifier ::= [
    ""keyword"" : ""@keyword"",
    ""keys"" : ""keys"",
    ""values"" : ""values"",
    default : key
]
";

        writeFile(tmpdir, "t.stg", templates);
        var group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));

        // try with mapped values
        var template = group.GetInstanceOf("t").Add("id", "keyword");
        Assert.AreEqual("@keyword", template.Render());

        // try with non-mapped values
        template = group.GetInstanceOf("t").Add("id", "nonkeyword");
        Assert.AreEqual("nonkeyword", template.Render());

        // try with non-mapped values that might break (Substring here guarantees unique instances)
        template = group.GetInstanceOf("t").Add("id", "_default".Substring(1));
        Assert.AreEqual("default", template.Render());

        template = group.GetInstanceOf("t").Add("id", "_keys".Substring(1));
        Assert.AreEqual("keys", template.Render());

        template = group.GetInstanceOf("t").Add("id", "_values".Substring(1));
        Assert.AreEqual("values", template.Render());
    }
}
