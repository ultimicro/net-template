namespace NetTemplate.Extensions;

using System.Reflection;

internal static class PropertyInfoExtensions
{
    public static MethodInfo GetGetMethod(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetMethod;
    }
}
