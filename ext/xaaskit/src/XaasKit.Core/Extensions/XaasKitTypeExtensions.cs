using JetBrains.Annotations;
using XaasKit.Core;

namespace System;

public static class AbpTypeExtensions
{
    public static string GetFullNameWithAssemblyName(this Type type)
    {
        return type.FullName + ", " + type.Assembly.GetName().Name;
    }

    /// <summary>
    /// Determines whether an instance of this type can be assigned to
    /// an instance of the <typeparamref name="TTarget"></typeparamref>.
    ///
    /// Internally uses <see cref="Type.IsAssignableFrom"/>.
    /// </summary>
    /// <typeparam name="TTarget">Target type</typeparam> (as reverse).
    public static bool IsAssignableTo<TTarget>(this Type type)
    {
        Check.NotNull(type, nameof(type));

        return type.IsAssignableTo(typeof(TTarget));
    }

    /// <summary>
    /// Determines whether an instance of this type can be assigned to
    /// an instance of the <paramref name="targetType"></paramref>.
    ///
    /// Internally uses <see cref="Type.IsAssignableFrom"/> (as reverse).
    /// </summary>
    /// <param name="type">this type</param>
    /// <param name="targetType">Target type</param>
    public static bool IsAssignableTo(this Type type, Type targetType)
    {
        Check.NotNull(type, nameof(type));
        Check.NotNull(targetType, nameof(targetType));

        return targetType.IsAssignableFrom(type);
    }

    /// <summary>
    /// Gets all base classes of this type.
    /// </summary>
    /// <param name="type">The type to get its base classes.</param>
    /// <param name="includeObject">True, to include the standard <see cref="object"/> type in the returned array.</param>
    public static Type[] GetBaseClasses(this Type type, bool includeObject = true)
    {
        Check.NotNull(type, nameof(type));

        var types = new List<Type>();
        AddTypeAndBaseTypesRecursively(types, type.BaseType, includeObject);
        return types.ToArray();
    }

    /// <summary>
    /// Gets all base classes of this type.
    /// </summary>
    /// <param name="type">The type to get its base classes.</param>
    /// <param name="stoppingType">A type to stop going to the deeper base classes. This type will be be included in the returned array</param>
    /// <param name="includeObject">True, to include the standard <see cref="object"/> type in the returned array.</param>
    public static Type[] GetBaseClasses(this Type type, Type stoppingType, bool includeObject = true)
    {
        Check.NotNull(type, nameof(type));

        var types = new List<Type>();
        AddTypeAndBaseTypesRecursively(types, type.BaseType, includeObject, stoppingType);
        return types.ToArray();
    }

    private static void AddTypeAndBaseTypesRecursively(
        ICollection<Type> types,
        Type? type,
        bool includeObject,
        Type? stoppingType = null)
    {
        if (type == null || type == stoppingType)
        {
            return;
        }

        if (!includeObject && type == typeof(object))
        {
            return;
        }

        AddTypeAndBaseTypesRecursively(types, type.BaseType, includeObject, stoppingType);
        types.Add(type);
    }
}