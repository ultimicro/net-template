namespace NetTemplate.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTemplate.Misc;
using Path = System.IO.Path;

[TestClass]
public class TestModelAdaptors : BaseTest
{
    private class UserAdaptor : IModelAdaptor
    {
        public object GetProperty(Interpreter interpreter, TemplateFrame frame, object o, object property, string propertyName)
        {
            if (propertyName.Equals("id"))
                return ((User)o).id;
            if (propertyName.Equals("name"))
                return ((User)o).Name;
            throw new TemplateNoSuchPropertyException(null, "User." + propertyName);
        }
    }

    private class UserAdaptorConst : IModelAdaptor
    {
        public object GetProperty(Interpreter interpreter, TemplateFrame frame, object o, object property, string propertyName)
        {
            if (propertyName.Equals("id"))
                return "const id value";
            if (propertyName.Equals("name"))
                return "const name value";
            throw new TemplateNoSuchPropertyException(null, "User." + propertyName);
        }
    }

    private class SuperUser : User
    {
#pragma warning disable 414 // The field 'name' is assigned but its value is never used
        private readonly int bitmask;
#pragma warning restore 414

        public SuperUser(int id, string name)
            : base(id, name)
        {
            bitmask = 0x8080;
        }

        public override string Name
        {
            get
            {
                return "super " + base.Name;
            }
        }
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSimpleAdaptor()
    {
        string templates =
                "foo(x) ::= \"<x.id>: <x.name>\"\n";
        writeFile(tmpdir, "foo.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "foo.stg"));
        group.RegisterModelAdaptor(typeof(User), new UserAdaptor());
        Template st = group.GetInstanceOf("foo");
        st.Add("x", new User(100, "parrt"));
        string expecting = "100: parrt";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestAdaptorAndBadProp()
    {
        ErrorBufferAllErrors errors = new ErrorBufferAllErrors();
        string templates =
                "foo(x) ::= \"<x.qqq>\"\n";
        writeFile(tmpdir, "foo.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "foo.stg"));
        group.Listener = errors;
        group.RegisterModelAdaptor(typeof(User), new UserAdaptor());
        Template st = group.GetInstanceOf("foo");
        st.Add("x", new User(100, "parrt"));
        string expecting = "";
        string result = st.Render();
        Assert.AreEqual(expecting, result);

        TemplateRuntimeMessage msg = (TemplateRuntimeMessage)errors.Errors[0];
        TemplateNoSuchPropertyException e = (TemplateNoSuchPropertyException)msg.Cause;
        Assert.AreEqual("User.qqq", e.PropertyName);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestAdaptorCoversSubclass()
    {
        string templates =
                "foo(x) ::= \"<x.id>: <x.name>\"\n";
        writeFile(tmpdir, "foo.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "foo.stg"));
        group.RegisterModelAdaptor(typeof(User), new UserAdaptor());
        Template st = group.GetInstanceOf("foo");
        st.Add("x", new SuperUser(100, "parrt")); // create subclass of User
        string expecting = "100: super parrt";
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestWeCanResetAdaptorCacheInvalidatedUponAdaptorReset()
    {
        string templates =
                "foo(x) ::= \"<x.id>: <x.name>\"\n";
        writeFile(tmpdir, "foo.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "foo.stg"));
        group.RegisterModelAdaptor(typeof(User), new UserAdaptor());
        group.GetModelAdaptor(typeof(User)); // get User, SuperUser into cache
        group.GetModelAdaptor(typeof(SuperUser));

        group.RegisterModelAdaptor(typeof(User), new UserAdaptorConst());
        // cache should be reset so we see new adaptor
        Template st = group.GetInstanceOf("foo");
        st.Add("x", new User(100, "parrt"));
        string expecting = "const id value: const name value"; // sees UserAdaptorConst
        string result = st.Render();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    [TestCategory(TestCategories.ST4)]
    public void TestSeesMostSpecificAdaptor()
    {
        string templates =
                "foo(x) ::= \"<x.id>: <x.name>\"\n";
        writeFile(tmpdir, "foo.stg", templates);
        TemplateGroup group = new TemplateGroupFile(Path.Combine(tmpdir, "foo.stg"));
        group.RegisterModelAdaptor(typeof(User), new UserAdaptor());
        group.RegisterModelAdaptor(typeof(SuperUser), new UserAdaptorConst()); // most specific
        Template st = group.GetInstanceOf("foo");
        st.Add("x", new User(100, "parrt"));
        string expecting = "100: parrt";
        string result = st.Render();
        Assert.AreEqual(expecting, result);

        st.Remove("x");
        st.Add("x", new SuperUser(100, "parrt"));
        expecting = "const id value: const name value"; // sees UserAdaptorConst
        result = st.Render();
        Assert.AreEqual(expecting, result);
    }
}
