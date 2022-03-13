namespace NetTemplate.Misc;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NetTemplate.Extensions;
using ArgumentNullException = System.ArgumentNullException;
using FieldInfo = System.Reflection.FieldInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using Type = System.Type;

public class ObjectModelAdaptor : IModelAdaptor
{
    private static readonly Dictionary<Type, Dictionary<string, System.Func<object, object>>> _memberAccessors =
        new Dictionary<Type, Dictionary<string, System.Func<object, object>>>();

    public virtual object GetProperty(Interpreter interpreter, TemplateFrame frame, object o, object property, string propertyName)
    {
        if (o == null)
            throw new ArgumentNullException("o");

        Type c = o.GetType();
        if (property == null)
            throw new TemplateNoSuchPropertyException(o, string.Format("{0}.{1}", c.FullName, propertyName ?? "null"));

        object value;
        var accessor = FindMember(c, propertyName);
        if (accessor != null)
        {
            value = accessor(o);
        }
        else
        {
            throw new TemplateNoSuchPropertyException(o, string.Format("{0}.{1}", c.FullName, propertyName));
        }

        return value;
    }

    private static System.Func<object, object> FindMember(Type type, string name)
    {
        if (type == null)
            throw new ArgumentNullException("type");
        if (name == null)
            throw new ArgumentNullException("name");

        lock (_memberAccessors)
        {
            Dictionary<string, System.Func<object, object>> members;
            System.Func<object, object> accessor = null;

            if (_memberAccessors.TryGetValue(type, out members))
            {
                if (members.TryGetValue(name, out accessor))
                    return accessor;
            }
            else
            {
                members = new Dictionary<string, System.Func<object, object>>();
                _memberAccessors[type] = members;
            }

            // must look up using reflection
            string methodSuffix = char.ToUpperInvariant(name[0]) + name.Substring(1);
            bool checkOriginalName = !string.Equals(methodSuffix, name);

            MethodInfo method = null;
            if (method == null)
            {
                PropertyInfo p = type.GetProperty(methodSuffix);
                if (p == null && checkOriginalName)
                    p = type.GetProperty(name);

                if (p != null)
                    method = p.GetGetMethod();
            }

            if (method == null)
            {
                method = type.GetMethod("Get" + methodSuffix, Type.EmptyTypes);
                if (method == null && checkOriginalName)
                    method = type.GetMethod("Get" + name, Type.EmptyTypes);
            }

            if (method == null)
            {
                method = type.GetMethod("get_" + methodSuffix, Type.EmptyTypes);
                if (method == null && checkOriginalName)
                    method = type.GetMethod("get_" + name, Type.EmptyTypes);
            }

            if (method != null)
            {
                accessor = BuildAccessor(method);
            }
            else
            {
                // try for an indexer
                method = type.GetMethod("get_Item", new Type[] { typeof(string) });
                if (method == null)
                {
                    var property = type.GetProperties().FirstOrDefault(IsIndexer);
                    if (property != null)
                        method = property.GetGetMethod();
                }

                if (method != null)
                {
                    accessor = BuildAccessor(method, name);
                }
                else
                {
                    // try for a visible field
                    FieldInfo field = type.GetField(name);
                    // also check .NET naming convention for fields
                    if (field == null)
                        field = type.GetField("_" + name);

                    if (field != null)
                        accessor = BuildAccessor(field);
                }
            }

            members[name] = accessor;

            return accessor;
        }
    }

    private static bool IsIndexer(PropertyInfo propertyInfo)
    {
        if (propertyInfo == null)
            throw new ArgumentNullException("propertyInfo");

        var indexParameters = propertyInfo.GetIndexParameters();
        return indexParameters != null
            && indexParameters.Length > 0
            && indexParameters[0].ParameterType == typeof(string);
    }

    private static System.Func<object, object> BuildAccessor(MethodInfo method)
    {
        ParameterExpression obj = Expression.Parameter(typeof(object), "obj");
        UnaryExpression instance = !method.IsStatic ? Expression.Convert(obj, method.DeclaringType) : null;
        Expression<System.Func<object, object>> expr = Expression.Lambda<System.Func<object, object>>(
            Expression.Convert(
                Expression.Call(instance, method),
                typeof(object)),
            obj);

        return expr.Compile();
    }

    /// <summary>
    /// Builds an accessor for an indexer property that returns a takes a string argument.
    /// </summary>
    private static System.Func<object, object> BuildAccessor(MethodInfo method, string argument)
    {
        ParameterExpression obj = Expression.Parameter(typeof(object), "obj");
        UnaryExpression instance = !method.IsStatic ? Expression.Convert(obj, method.DeclaringType) : null;
        Expression<System.Func<object, object>> expr = Expression.Lambda<System.Func<object, object>>(
            Expression.Convert(
                Expression.Call(instance, method, Expression.Constant(argument)),
                typeof(object)),
            obj);

        return expr.Compile();
    }

    private static System.Func<object, object> BuildAccessor(FieldInfo field)
    {
        ParameterExpression obj = Expression.Parameter(typeof(object), "obj");
        UnaryExpression instance = !field.IsStatic ? Expression.Convert(obj, field.DeclaringType) : null;
        Expression<System.Func<object, object>> expr = Expression.Lambda<System.Func<object, object>>(
            Expression.Convert(
                Expression.Field(instance, field),
                typeof(object)),
            obj);

        return expr.Compile();
    }
}
