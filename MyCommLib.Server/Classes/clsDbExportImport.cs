namespace MyCommLib.Server.Classes;

using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MyCommLib.Classes;
using MyCommLib.Shared.Models;
using System.Data;
using System.Reflection;
public class clsDbExportImport<TEntity> where TEntity : class {
    private readonly DbTableModel dbt;
    private readonly DbContext dc;
    public clsDbExportImport(DbTableModel dbt, DbContext dc) {
        this.dbt = dbt;
        this.dc = dc;
    }
    public string TableName => typeof(TEntity).Name;
    public DataTable? Dt { get; set; }
    public DataTable GetDt() {
        if (Dt == null) GetObjsFromExcel();
        return Dt!;
    }
    public DataTable GetDt(int max) {
        if (Dt == null) GetObjsFromExcel();
        if (Dt!.Rows.Count > max) // if there are more than <max> rows, return top <max> rows
        {
            var dt = new DataTable();
            foreach (var col in Dt.Columns) {
                dt.Columns.Add(col.ToString());
            }
            for (int i = 0; i < max; i++) {
                dt.Rows.Add();
                int iCol = 0;
                foreach (var col in Dt.Columns) {
                    dt.Rows[i][iCol] = Dt.Rows[i][iCol].ToString();
                    iCol++;
                }
            }
            return dt;
        }
        return Dt;
    }
    public List<TEntity> Objects { get; set; } = [];
    private string GetPathJson => Path.Combine(clsDbExportImport<TEntity>.GetPath(Path.Combine(Environment.CurrentDirectory, "App_Data", "DbExports")), $"{TableName}.json");
    private string GetPathExcel => Path.Combine(clsDbExportImport<TEntity>.GetPath(Path.Combine(Environment.CurrentDirectory, "App_Data", "DbExports")), $"{TableName}.xlsx");
    private static string GetPath(string path) {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        return path;
    }
    public void DeleteAll() {
        var list = dc.Set<TEntity>().ToList();
        if (list.Count != 0) dc.RemoveRange(list);
    }
    public void SetCount() {
        dbt.CountDb = dc.Set<TEntity>().Count();
        dbt.CountFile = GetObjsFromFile().Count();
        dbt.CountExcel = GetObjsFromExcel().Count();
    }
    public IEnumerable<TEntity> GetObjsFromFile() {
        _ = new List<TEntity>();
        List<TEntity>? objs;
        try {
            var jsonString = File.ReadAllText(GetPathJson);
            dbt.JsonFile = jsonString; // for downloading data

            var json = new clsJson<TEntity>();
            objs = json.GetList(jsonString);
        } catch (Exception ex) {
            throw new Exception(ex.Message);
        }
        return objs!;
    }
    public IEnumerable<TEntity> GetObjsFromExcel() {
        var objs = new List<TEntity>();
        if (!File.Exists(GetPathExcel)) return objs;
        Type table = typeof(TEntity);
        PropertyInfo[] properties = table.GetProperties();
        try {
            MemoryStream ms = new(File.ReadAllBytes(GetPathExcel));
            dbt.ExcelData = ms.ToArray(); // for downloading data
            Dt = new DataTable();
            using (var xls = new clsExcel()) {
                xls.Open(ms);
                Dt = xls.GetDataTable();
            }
            ms.Flush();
            ms.Dispose();
        } catch (Exception ex) {
            throw new Exception(ex.Message);
        }
        foreach (DataRow row in Dt.Rows) {
            TEntity obj = Activator.CreateInstance<TEntity>()!;
            foreach (var property in properties) {
                var type = property.PropertyType.Name.ToLower();
                if (!property.CanWrite) { }
                //else if (property.Name == "IsAdmin") { }
                else {
                    try {
                        if (type == "string") {
                            property.SetValue(obj, row[property.Name].ToString());
                        } else if (type == "int32") {
                            var val = int.Parse(row[property.Name].ToString() ?? "");
                            property.SetValue(obj, val);
                        } else if (type == "int64") {
                            var val = long.Parse(row[property.Name].ToString() ?? "");
                            property.SetValue(obj, val);
                        } else if (type == "datetime") {
                            if (!string.IsNullOrEmpty(row[property.Name].ToString())) {
                                var val = DateTime.Parse(row[property.Name].ToString() ?? "");
                                property.SetValue(obj, val);
                            }
                        } else if (type == "boolean") {
                            _ = bool.TryParse(row[property.Name].ToString(), out bool val);
                            property.SetValue(obj, val);
                        } else if (type == "decimal") {
                            _ = decimal.TryParse(row[property.Name].ToString(), out decimal val);
                            property.SetValue(obj, val);
                        } else if (type == "guid") {
                            var str = row[property.Name].ToString() ?? "";
                            var guid = Guid.Parse(str);
                            property.SetValue(obj, guid);
                        }
                    } catch (Exception ex) {
                        Console.WriteLine($"Something went wrong! {ex}");
                    }
                }
            }
            objs.Add(obj);
        }
        return objs;
    }
    public void ExportToFile() {
        Objects = [.. dc.Set<TEntity>()];
        var jsonString = new clsJson<List<TEntity>>().Serialize(Objects);
        File.WriteAllText(GetPathJson, jsonString);
        SetCount();
    }
    public async Task<bool> ExportToExcel() {
        await Task.CompletedTask;
        Objects = [.. dc.Set<TEntity>()];
        using (var xls = new clsExcel()) {
            if (!CreateExcel(xls)) return false;
            var msg = xls.SaveAs(GetPathExcel);
            if (!String.IsNullOrEmpty(msg)) {
                throw new Exception(msg);
            }
        }
        SetCount();
        return true;
    }
    private bool CreateExcel(clsExcel xls) {
        CreateTemplate();
        Type table = typeof(TEntity);
        PropertyInfo[] properties = table.GetProperties();

        if (!xls.OpenTemplate($"Db{TableName}.xlsx")) {
            throw new Exception($"Template not found! (Db{TableName}.xlsx)");
        }
        IXLWorksheet ws = xls.ws;

        var total = Objects.Count;
        var row = xls.CopyRowN("NextRow", total);
        foreach (var obj in Objects) {
            var col = 0;
            foreach (var property in properties) {
                if (property.CanWrite) {
                    col++;
                    if (property.PropertyType.Name == "DateTime") {
                        if ((DateTime)property.GetValue(obj)! != DateTime.MinValue) {
                            var x = ((DateTime)(property.GetValue(obj)!)).ToString("dd MMM yyyy HH:mm:ss");
                            ws.Cell(row, col).SetValue(x);
                        }
                    } else if (property.PropertyType.Name == "Decimal") {
                        var x = ((Decimal)(property.GetValue(obj)!)).ToString("#0.000000");
                        ws.Cell(row, col).SetValue(x);
                    } else if (property.PropertyType.Name == "String") {
                        ws.Cell(row, col).SetValue(property.GetValue(obj)?.ToString() ?? "");  // string items have to be set as string (not as number nor date)
                    } else if ((new List<string>() { "Int32", "Int64", "Boolean" }).Contains(property.PropertyType.Name)) {
                        //ws.Cell(row, col).Value = property.GetValue(obj);
                        ws.Cell(row, col).SetValue(property.GetValue(obj)?.ToString() ?? "");
                    } else if (property.PropertyType.Name == "Guid") {
                        ws.Cell(row, col).SetValue(property.GetValue(obj)?.ToString() ?? "");
                    }
                }
            }
            row++;
        }
        xls.DeleteRows("NextRow");
        return true;
    }
    public void CreateTemplate() {
        Type table = typeof(TEntity);
        PropertyInfo[] properties = table.GetProperties();
        using var xls = new clsExcel();
        var templatePath = xls.TemplatePath($"Db{table.Name}.xlsx");
        if (xls.OpenTemplate(templatePath)) return;
        if (!xls.OpenTemplate("DbExportImport")) {
            throw new Exception("Template not found! (DbExportImport.xlsx)");
        }
        IXLWorksheet ws = xls.ws;

        var targetTypes = new string[] { "String", "Int32", "Int64", "DateTime", "Boolean", "Decimal" };
        int col = 0;
        foreach (var property in properties) {
            if (!property.CanWrite) { } // such as calculated value
            else if (!targetTypes.Contains(property.PropertyType.Name)) { } // such as IEnumerable, Classes, etc.
            else {
                ws.Cell(1, ++col).Value = property.Name;
            }
        }
        xls.SaveAs(templatePath);
    }
    public void ImportFromFile() {
        DeleteAll();
        Objects = [.. GetObjsFromFile()];
        ImportToDatabase();
        SetCount();
    }
    public void ImportToDatabase() {
        DeleteAll();
        foreach (var obj in Objects) {
            TEntity newObj = Activator.CreateInstance<TEntity>()!;
            obj.CopyTo(newObj);
            dc.Add(newObj);
        }
        dc.SaveChanges();
    }
    public async Task<bool> DownloadExcel(IJSRuntime JS) {
        await Task.CompletedTask;
        using var xls = new clsExcel();
        if (!CreateExcel(xls)) return false;
        xls.Download($"{TableName}.xlsx", JS);
        return true;
    }

// ******************** (Old) *************************************************
    private Dictionary<long, long> IDConv = [];
    public void ImportToDatabase(Func<TEntity, TEntity> setNewId) // for tables which does not have child tables
    {
        DeleteAll();
        foreach (var obj in Objects) {
            TEntity newObj = Activator.CreateInstance<TEntity>()!;
            obj.CopyTo(newObj);
            setNewId(newObj); // Callback method, which convert old Id to new Id
            dc.Add(newObj);
        }
        dc.SaveChanges();
    }
    public void ImportToDatabase(string idName, Func<TEntity, TEntity> setNewId) // for tables which have child tables (Set IDConv table)
    {
        DeleteAll();
        Type table = typeof(TEntity);
        PropertyInfo[] properties = table.GetProperties();
        IDConv = [];

        foreach (var obj in Objects) {
            TEntity newObj = Activator.CreateInstance<TEntity>()!;
            clsObject.CopyTo<TEntity, TEntity>(obj, newObj);
            setNewId(newObj); // Callback method, which convert old Id to new Id
            dc.Add(newObj);
            dc.SaveChanges();

            foreach (var property in properties) {
                if (property.Name == idName) {
                    var oldId = Int32.Parse(property.GetValue(obj)!.ToString() ?? "");
                    var newId = Int32.Parse(property.GetValue(newObj)!.ToString() ?? "");
                    IDConv.Add(oldId, newId);
                }
            }
        }
    }
    public long GetNewId(long oldId) => IDConv[oldId];
}
