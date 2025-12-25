using System.Reflection;

namespace MyCommLib.Classes;
public static class clsObject
{
    public static void CopyTo<TFrom, TTo>(this TFrom from, TTo to)
    {
        CopyTo(from, to, x => false, false);
    }
    public static void CopyTo<TFrom, TTo>(this TFrom from, TTo to, bool excludekey)
    {
        CopyTo(from, to, x => excludekey && (x.Name == typeof(TFrom).Name + "Id" || x.Name == "Id"), false);
    }
    public static void CopyTo<TFrom, TTo>(this TFrom from, TTo to, bool excludekey, bool copyOnly)
    {
        CopyTo(from, to, x => excludekey && (x.Name == typeof(TFrom).Name + "Id" || x.Name == "Id"), copyOnly);
    }
    public static void CopyTo<TFrom, TTo>(this TFrom from, TTo to, Func<System.Reflection.PropertyInfo, bool> exclude, bool copyOnly)
    {
        if (from == null || to == null) return;
        var propsFrom = typeof(TFrom).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                         .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);
        var propsTo = typeof(TTo).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0)
                                     .ToDictionary(p => p.Name);
        foreach (var propFrom in propsFrom)
        {
            if (exclude(propFrom)) continue;
            if (!propsTo.TryGetValue(propFrom.Name, out var propTo)) continue;
            if (!propTo.PropertyType.IsAssignableFrom(propFrom.PropertyType)) continue;
            if (copyOnly && !IsSimpleType(propFrom.PropertyType)) continue;

            var value = propFrom.GetValue(from);
            propTo.SetValue(to, value);
        }
    }
    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime) || type == typeof(Guid);
    }

    public static bool EqualTo<TFrom, TTo>(this TFrom from, TTo to)
    {
        return EqualTo(from, to, x => false);
    }
    public static bool EqualTo<TFrom, TTo>(this TFrom from, TTo to, bool excludekey)
    {
        return EqualTo(from, to, x => excludekey && (x.Name == typeof(TFrom).Name + "Id" || x.Name == "Id"));
    }
    public static bool EqualTo<TFrom, TTo>(this TFrom from, TTo to, Func<System.Reflection.PropertyInfo, bool> exclude)
    {
        if (ReferenceEquals(from, to)) return true;
        if (from == null || to == null) return false;
        var propsFrom = typeof(TFrom).GetProperties().Where(x => x.CanRead);
        var propsTo = typeof(TTo).GetProperties().Where(x => x.CanRead).ToDictionary(x => x.Name);
        foreach (var propFrom in propsFrom)
        {
            if (exclude(propFrom)) continue;
            if (!propsTo.TryGetValue(propFrom.Name, out var propTo)) continue;
            var valFrom = propFrom.GetValue(from);
            var valTo = propTo.GetValue(to);
            if (!object.Equals(valFrom, valTo)) return false;
        }
        return true;
    }

}
