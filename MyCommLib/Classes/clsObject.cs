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
        Type type = typeof(TFrom);
        PropertyInfo[] properties = type.GetProperties();
        string[] types =["String", "Int32", "Int64", "DateTime", "Boolean", "Decimal", "Byte", "SByte", "Char", "Double", "Single" ];

        var propsFrom = typeof(TFrom).GetProperties().Where(x => x.CanRead).ToList();
        var propsTo = typeof(TTo).GetProperties().Where(x => x.CanWrite).ToList();
        foreach (var propFrom in propsFrom)
        {
            var name = propFrom.Name;
            //Console.WriteLine(name);
            var propTo = propsTo.FirstOrDefault(x => x.Name == propFrom.Name);
            if (propTo is null) { }
            else if (exclude(propFrom)) { }
            else
            {
                if (!copyOnly || types.Contains(propFrom.PropertyType.Name))
                {
                    //Console.WriteLine(propFrom.PropertyType.Name);
                    //propTo.SetValue(to, propFrom.GetValue(from));
                    var value = propFrom.GetValue(from);
                    propTo.SetValue(to, value);
                }
            }
        }
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
        var propsFrom = typeof(TFrom).GetProperties().Where(x => x.CanRead).ToList();
        var propsTo = typeof(TTo).GetProperties().Where(x => x.CanWrite).ToList();
        foreach (var propFrom in propsFrom)
        {
            var propTo = propsTo.FirstOrDefault(x => x.Name == propFrom.Name);
            if (propTo is null) { }
            else if (exclude(propFrom)) { }
            else
            {
                var valFrom = propFrom.GetValue(from)?.ToString();
                var valTo = propTo.GetValue(to)?.ToString();
                if (valFrom != valTo)
                {
                    return false;
                }
            }
        }
        return true;
    }

}
