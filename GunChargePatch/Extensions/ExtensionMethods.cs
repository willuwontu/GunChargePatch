using System;
using System.Linq.Expressions;
using System.Reflection;

public static class ExtensionMethods
{
    #region Reflection

    // methods
    public static MethodInfo GetMethodInfo(Type type, string methodName)
    {
        MethodInfo methodInfo = null;
        do
        {
            methodInfo = type.GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
            type = type.BaseType;
        }
        while (methodInfo == null && type != null);
        return methodInfo;
    }
    public static object InvokeMethod(this object obj, string methodName, params object[] arguments)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");
        Type objType = obj.GetType();
        MethodInfo propInfo = GetMethodInfo(objType, methodName);
        if (propInfo == null)
            throw new ArgumentOutOfRangeException("propertyName",
                string.Format("Couldn't find property {0} in type {1}", methodName, objType.FullName));
        return propInfo.Invoke(obj, arguments);
    }
    public static MethodInfo GetMethodInfo(Type type, string methodName, Type[] parameters)
    {
        MethodInfo methodInfo = null;
        do
        {
            methodInfo = type.GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly,
                null,
                parameters,
                null);
            type = type.BaseType;
        }
        while (methodInfo == null && type != null);
        return methodInfo;
    }
    public static object InvokeMethod(this object obj, string methodName, Type[] argumentOrder, params object[] arguments)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");
        Type objType = obj.GetType();
        MethodInfo propInfo = GetMethodInfo(objType, methodName, argumentOrder);
        if (propInfo == null)
            throw new ArgumentOutOfRangeException("propertyName",
                string.Format("Couldn't find property {0} in type {1}", methodName, objType.FullName));
        return propInfo.Invoke(obj, arguments);
    }

    // fields
    public static FieldInfo GetFieldInfo(Type type, string fieldName)
    {
        FieldInfo fieldInfo = null;
        do
        {
            fieldInfo = type.GetField(fieldName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            type = type.BaseType;
        }
        while (fieldInfo == null && type != null);
        return fieldInfo;
    }
    public static object GetFieldValue(this object obj, string fieldName)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");
        Type objType = obj.GetType();
        FieldInfo propInfo = GetFieldInfo(objType, fieldName);
        if (propInfo == null)
            throw new ArgumentOutOfRangeException("propertyName",
                string.Format("Couldn't find property {0} in type {1}", fieldName, objType.FullName));
        return propInfo.GetValue(obj);
    }
    public static void SetFieldValue(this object obj, string fieldName, object val)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");
        Type objType = obj.GetType();
        FieldInfo propInfo = GetFieldInfo(objType, fieldName);
        if (propInfo == null)
            throw new ArgumentOutOfRangeException("propertyName",
                string.Format("Couldn't find property {0} in type {1}", fieldName, objType.FullName));
        propInfo.SetValue(obj, val);
    }

    // properties
    public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
    {
        PropertyInfo propInfo = null;
        do
        {
            propInfo = type.GetProperty(propertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            type = type.BaseType;
        }
        while (propInfo == null && type != null);
        return propInfo;
    }
    public static object GetPropertyValue(this object obj, string propertyName)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");
        Type objType = obj.GetType();
        PropertyInfo propInfo = GetPropertyInfo(objType, propertyName);
        if (propInfo == null)
            throw new ArgumentOutOfRangeException("propertyName",
                string.Format("Couldn't find property {0} in type {1}", propertyName, objType.FullName));
        return propInfo.GetValue(obj, null);
    }
    public static void SetPropertyValue(this object obj, string propertyName, object val)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");
        Type objType = obj.GetType();
        PropertyInfo propInfo = GetPropertyInfo(objType, propertyName);
        if (propInfo == null)
            throw new ArgumentOutOfRangeException("propertyName",
                string.Format("Couldn't find property {0} in type {1}", propertyName, objType.FullName));
        propInfo.SetValue(obj, val, null);
    }

    // casting
    public static object Cast(this Type Type, object data)
    {
        var DataParam = Expression.Parameter(typeof(object), "data");
        var Body = Expression.Block(Expression.Convert(Expression.Convert(DataParam, data.GetType()), Type));

        var Run = Expression.Lambda(Body, DataParam).Compile();
        var ret = Run.DynamicInvoke(data);
        return ret;
    }

    #endregion
}
