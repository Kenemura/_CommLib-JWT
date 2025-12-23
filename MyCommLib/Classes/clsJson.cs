namespace MyCommLib.Classes;

using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

public class clsJson<T> where T : class {
    public clsJson() { }
    public string Serialize(T obj) {
        var json = JsonSerializer.Serialize<T>(obj, JsonOpts());
        return json;
    }
    public T Deserialize(string json) {
        var obj = JsonSerializer.Deserialize<T>(json, JsonOpts());
        return obj!;
    }
    public List<T> DeserializeList(string json) {
        var objs = JsonSerializer.Deserialize<List<T>>(json, JsonOpts());
        return objs!;
    }
    public string Serialize(IEnumerable<T> objs) {
        var json = JsonSerializer.Serialize<IEnumerable<T>>(objs, JsonOpts());
        return json;
    }
    public List<T> GetList(string json) => DeserializeList(json);
    public T GetObj(string json) => Deserialize(json);

    private JsonSerializerOptions JsonOpts() {
        return new JsonSerializerOptions {
            ReferenceHandler = ReferenceHandler.Preserve,
            WriteIndented = true
        };
    }

    public DataTable GetDataTable(string json) {
        var dt = new DataTable();
        var objs = DeserializeList(json);
        var props = typeof(T).GetProperties().Where(x => x.CanWrite);
        var first = true;
        foreach (var obj in objs) {
            if (first) {
                foreach (var prop in props) {
                    dt.Columns.Add(prop.Name);
                }
                first = false;
            }
            var row = dt.NewRow();
            int col = 0;
            foreach (var prop in props) {
                row[col++] = prop.GetValue(obj);
            }
            dt.Rows.Add(row);
        }
        return dt;
    }
}
