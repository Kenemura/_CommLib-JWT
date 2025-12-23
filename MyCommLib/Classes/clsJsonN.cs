namespace MyCommLib.Classes;

using Newtonsoft.Json;
using System.Data;

public static class clsJsonN {
    public static string Serialize<T>(T obj) where T : class {
        var json = JsonConvert.SerializeObject(obj);
        return json;
    }
    public static T Deserialize<T>(string json) where T : class {
        var obj = JsonConvert.DeserializeObject<T>(json);
        return obj!;
    }
    public static List<T> DeserializeList<T>(string json) where T : class {
        var obj = JsonConvert.DeserializeObject<List<T>>(json);
        return obj!;
    }
    public static string ToJsonN(this object obj) => JsonConvert.SerializeObject(obj);

    public static DataTable GetDataTable<T>(string json) where T : class {
        var dt = new DataTable();
        var objs = Deserialize<List<T>>(json);
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
