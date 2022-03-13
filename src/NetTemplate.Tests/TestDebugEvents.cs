namespace NetTemplate.Tests;

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTemplate.Debug;
using NetTemplate.Tests.Extensions;
using Environment = System.Environment;
using Path = System.IO.Path;
using StringWriter = System.IO.StringWriter;

[TestClass]
public class TestDebugEvents : BaseTest
{
    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestString()
    {
        string templates =
            "t() ::= <<foo>>" + Environment.NewLine;

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        Template st = group.GetInstanceOf("t");
        List<InterpEvent> events = st.GetEvents();
        string expected =
            "[EvalExprEvent{self=/t(), expr='foo', source=[0..3), output=[0..3)}," +
            " EvalTemplateEvent{self=/t(), output=[0..3)}]";
        string result = events.ToListString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestAttribute()
    {
        string templates =
            "t(x) ::= << <x> >>" + Environment.NewLine;

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        Template st = group.GetInstanceOf("t");
        List<InterpEvent> events = st.GetEvents();
        string expected =
            "[IndentEvent{self=/t(x), expr=' <x>', source=[0..4), output=[0..1)}," +
            " EvalExprEvent{self=/t(x), expr='<x>', source=[1..4), output=[0..0)}," +
            " EvalExprEvent{self=/t(x), expr=' ', source=[4..5), output=[0..1)}," +
            " EvalTemplateEvent{self=/t(x), output=[0..1)}]";
        string result = events.ToListString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestTemplateCall()
    {
        string templates =
            "t(x) ::= <<[<u()>]>>\n" +
            "u() ::= << <x> >>\n";

        writeFile(tmpdir, "t.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "t.stg"));
        Template st = group.GetInstanceOf("t");
        List<InterpEvent> events = st.GetEvents();
        string expected =
            "[EvalExprEvent{self=/t(x), expr='[', source=[0..1), output=[0..1)}," +
            " IndentEvent{self=/u(), expr=' <x>', source=[0..4), output=[1..2)}," +
            " EvalExprEvent{self=/u(), expr='<x>', source=[1..4), output=[1..1)}," +
            " EvalExprEvent{self=/u(), expr=' ', source=[4..5), output=[1..2)}," +
            " EvalTemplateEvent{self=/u(), output=[1..2)}," +
            " EvalExprEvent{self=/t(x), expr='<u()>', source=[1..6), output=[1..2)}," +
            " EvalExprEvent{self=/t(x), expr=']', source=[6..7), output=[2..3)}," +
            " EvalTemplateEvent{self=/t(x), output=[0..3)}]";
        string result = events.ToListString();
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestEvalExprEventForSpecialCharacter()
    {
        string templates = "t() ::= <<[<\\n>]>>\n";
        //                            012 345
        TemplateGroupString g = new TemplateGroupString(templates);
        Template st = g.GetInstanceOf("t");
        st.impl.Dump();
        StringWriter writer = new StringWriter();
        List<InterpEvent> events = st.GetEvents(new AutoIndentWriter(writer, "\n"));
        string expected =
            "[EvalExprEvent{self=/t(), expr='[', source=[0..1), output=[0..1)}, " +
            "EvalExprEvent{self=/t(), expr='\\n', source=[2..4), output=[1..2)}, " +
            "EvalExprEvent{self=/t(), expr=']', source=[5..6), output=[2..3)}, " +
            "EvalTemplateEvent{self=/t(), output=[0..3)}]";
        string result = events.ToListString();
        Assert.AreEqual(expected, result);
    }
}
