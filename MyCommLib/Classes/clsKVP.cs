namespace MyCommLib.Classes;
public class clsKVP
{
    public clsKVP(string key, string value)
    {
        Key = key;
        Value = value;
    }
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
}
