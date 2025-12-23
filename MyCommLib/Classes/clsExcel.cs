using ClosedXML.Excel;
using Microsoft.JSInterop;
using System.Data;
using System.Reflection;

namespace MyCommLib.Classes;
public class clsExcel : IDisposable
{
    private XLWorkbook _wb = null!;
    private IXLWorksheet _ws = null!;
    public XLWorkbook wb => _wb;
    public IXLWorksheet ws => _ws;
    public string ContentRoot { get; set; } = "";
    public string TemplatePath(string name) => Path.Combine(Environment.CurrentDirectory, "App_Data", "Dots", name);
    public string OutputPath(string name) => Path.Combine(Environment.CurrentDirectory, "App_Data", "Outputs", name);
    public static string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public clsExcel() { }
    public bool Open(string path)
    {
        if (Path.GetExtension(path) != ".xlsx") { return false; }
        if (!File.Exists(path)) { return false; }
        _wb = new XLWorkbook(path);
        _ws = _wb.Worksheet(1);
        return true;
    }
    public bool Open(MemoryStream ms)
    {
        _wb = new XLWorkbook(ms);
        _ws = _wb.Worksheet(1);
        return true;
    }
    public bool Open(byte[] bs)
    {
        try
        {
            MemoryStream ms = new MemoryStream(bs);
            //MemoryStream ms = new MemoryStream();
            //ms.SetLength(bs.Length);
            //ms.Write(bs, 0, bs.Length);
            //ms.Position = 0;
            _wb = new XLWorkbook(ms);
            _ws = _wb.Worksheet(1);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        return true;
    }
    public bool OpenTemplate(string name)
    {
        string path = TemplatePath(name);
        if (Path.GetExtension(path) != ".xlsx") path += ".xlsx";
        return Open(path);
    }
    public void Download(string filename, IJSRuntime js)
    {
        if (Path.GetExtension(filename) != ".xlsx") filename += ".xlsx";
        js.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(this.GetBytes()));
    }
    public void Download(string filename, IJSRuntime js, byte[] filedata)
    {
        if (Path.GetExtension(filename) != ".xlsx") filename += ".xlsx";
        js.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(filedata));
    }
    public byte[] GetBytes()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            _wb.SaveAs(ms);
            return ms.ToArray();
        }
    }
    public string SaveAs(string path)
    {
        try
        {
            _wb.SaveAs(path);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return "";
    }
    public DataTable GetDataTable()
    {
        DataTable dt = new DataTable();
        _ws = _wb.Worksheet(1);
        bool firstRow = true;
        string readRange = "1:1";
        foreach (IXLRow row in _ws.RowsUsed())
        {
            if (firstRow)
            {
                readRange = $"1:{row.LastCellUsed().Address.ColumnNumber}";
                foreach (IXLCell cell in row.Cells(readRange))
                {
                    dt.Columns.Add(cell.Value.ToString());
                }
                firstRow = false;
            }
            else
            {
                dt.Rows.Add();
                int cellIndex = 0;
                foreach (IXLCell cell in row.Cells(readRange))
                {
                    dt.Rows[dt.Rows.Count - 1][cellIndex] = cell.Value.ToString();
                    cellIndex++;
                }
            }
        }
        return dt;
    }
    public IEnumerable<T> GetObjects<T>() where T : class
    {
        var objs = new List<T>();
        var colItems = new Dictionary<string, int>();
        var props = typeof(T).GetProperties().Where(x => x.CanWrite);
        foreach (var prop in props)
        {
            colItems.Add(prop.Name, 0);
        }

        _ws = _wb.Worksheet(1);
        bool firstRow = true;
        string readRange = "1:1";
        foreach (IXLRow row in _ws.RowsUsed())
        {
            if (firstRow)
            {
                readRange = $"1:{row.LastCellUsed().Address.ColumnNumber}";
                foreach (IXLCell cell in row.Cells(readRange))
                {
                    try
                    {
                        colItems[cell.Value.ToString() ?? ""] = cell.Address.ColumnNumber;
                    }
                    catch { }
                }
                firstRow = false;
            }
            else
            {
                var obj = (T)Activator.CreateInstance(typeof(T))!;
                foreach (var prop in props)
                {
                    var col = colItems[prop.Name];
                    if (col > 0)
                    {
                        try
                        {
                            var type = prop.PropertyType.Name;
                            var cellData = row.Cell(col).Value;
                            var cellText = cellData.ToString() ?? "";
                            var cell = row.Cell(col);
                            if (type == "Int32")
                            {
                                prop.SetValue(obj, cell.GetValue<int>());
                            }
                            else if (type == "Int64")
                            {
                                prop.SetValue(obj, cell.GetValue<long>());
                            }
                            else if (type == "Decimal")
                            {
                                prop.SetValue(obj, cell.GetValue<Decimal>());
                            }
                            else if (type == "String")
                            {
                                prop.SetValue(obj, cell.GetString());
                            }
                            else if (type == "DateTime")
                            {
                                prop.SetValue(obj, cell.GetDateTime());
                            }
                            else if (type == "Boolean")
                            {
                                prop.SetValue(obj, cell.GetBoolean());
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        Console.WriteLine($"{prop.Name} {col}");
                    }
                }
                objs.Add(obj);
            }
        }
        return objs;
    }
    public void LoadObjects<T>(IEnumerable<T> objs)
    {
        var props = typeof(T).GetProperties().Where(x => x.CanWrite);
        var cols = new Dictionary<string, int>();
        _ws = _wb.Worksheet(1);
        var firstRow = _ws.Row(1);
        int i = 1;
        foreach (var cell in firstRow.Cells($"1:{firstRow.LastCellUsed().Address.ColumnNumber}"))
        {
            cols.Add(cell.Value.ToString() ?? "", i++);
        }
        var row = CopyRowN("NextRow", objs.Count());
        foreach (var obj in objs)
        {
            foreach (var prop in props)
            {
                if (cols.ContainsKey(prop.Name))
                {
                    var col = cols[prop.Name];
                    this.SetCell<T>(_ws.Cell(row, col), prop, obj);
                }
            }
            row++;
        }
        DeleteRows("NextRow");
    }

    public void SetCell<T>(IXLCell cell, PropertyInfo prop, T obj)
    {
        if (prop.PropertyType.Name == "DateTime")
        {
            var dateTime = (DateTime)(prop.GetValue(obj) ?? DateTime.MinValue);
            if ((dateTime != DateTime.MinValue))
            {
                cell.SetValue(dateTime);
            }
            else
            {
                cell.SetValue("");
            }
        }
        else if (prop.PropertyType.Name == "Decimal")
        {
            var x = ((Decimal)(prop.GetValue(obj) ?? 0M));
            cell.SetValue(x);
        }
        else if (prop.PropertyType.Name == "String")
        {
            cell.SetValue(prop.GetValue(obj)?.ToString() ?? "");  // string items have to be set as string (not as number nor date)
        }
        else if (prop.PropertyType.Name == "Int32")
        {
            cell.SetValue((int)(prop.GetValue(obj) ?? 0));
        }
        else if (prop.PropertyType.Name == "Int64")
        {
            cell.SetValue((long)(prop.GetValue(obj) ?? 0));
        }
        else if (prop.PropertyType.Name == "Boolean")
        {
            cell.SetValue((bool)(prop.GetValue(obj) ?? false));
        }
    }
    public bool GotoSheet(string name)
    {
        foreach(IXLWorksheet ws in _wb.Worksheets)
        {
            _ws = ws;
            return true;
        }
        return false;
    }
    public int CopyRow(string name)
    {
        IXLRange rng = _ws.Range(name);
        int r = RowFrom(rng);
        _ws.Row(r).InsertRowsAbove(rng.RowCount());
        //_ws.Cell(r, 1).Value = rng;
        _ws.Cell(r, 1).CopyFrom(rng);
        return r;
    }
    public int CopyRowN(string name, int n)
    {
        IXLRange rng = _ws.Range(name);
        int r = RowFrom(rng);
        if (n > 0) _ws.Row(r).InsertRowsAbove(rng.RowCount() * n);
        for (int i = 0; i < n; i++)
        {
            _ws.Cell(r + rng.RowCount() * i, 1).CopyFrom(rng);
        }
        return r;
    }
    public void DeleteRows(string name)
    {
        var rng = _ws.Range(name);
        var rngAdr = rng.RangeAddress;
        rng.Delete(XLShiftDeletedCells.ShiftCellsUp);

        rng = _ws.Range(rngAdr);
        rng.Clear();
        rng.Style.Fill.BackgroundColor = XLColor.NoColor;
        DeleteName(name);
    }
    public void DeleteName(string name) // Delete defined name
    {
        _wb.NamedRange(name).Delete();  // for ver 0.102
        //_wb.DefinedNames.Delete(name); // for ver 0.104
    }
    public int RowFrom(IXLRange rng) => rng.RangeAddress.FirstAddress.RowNumber;
    public int RowTo(IXLRange rng) => rng.RangeAddress.LastAddress.RowNumber;
    public int ColFrom(IXLRange rng) => rng.RangeAddress.FirstAddress.ColumnNumber;
    public int ColTo(IXLRange rng) => rng.RangeAddress.LastAddress.ColumnNumber;

    // Dispose
    ~clsExcel()
    {
        if (disposed) return;
        Dispose(false);
    }
    bool disposed = false;
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;
        if (disposing)
        {
            if (_wb != null) _wb.Dispose();
        }
        disposed = true;
    }
}
