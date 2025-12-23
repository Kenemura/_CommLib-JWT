namespace MyCommLib.Shared.Models;
public class DbTableModel
{
    public string Name { get; set; } = "";
    public int CountDb { get; set; } = 0;
    public int CountFile { get; set; } = 0;
    public int CountExcel { get; set; } = 0;
    public string JsonFile { get; set; } = "";
    public byte[] ExcelData { get; set; } = new byte[0];
}
