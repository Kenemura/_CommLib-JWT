namespace MyCommLib.Classes;

using System.Text.Json;
using System.Text.Json.Serialization;

public static class clsDebuggingExtentions
{
    public static JsonSerializerOptions opts = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve };
    public static string ToJson(this object obj) => JsonSerializer.Serialize(obj, opts);

    public static JsonSerializerOptions opts2 = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.IgnoreCycles };
    public static string ToJson(this object obj, bool preserve = true) =>
        preserve ? JsonSerializer.Serialize(obj, opts)
        : JsonSerializer.Serialize(obj, opts2);
}
