namespace MyCommLib.Classes;
public class clsListItem
{
    public string Item { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Align { get; set; } = ""; // "", "R", "C"
    public int Width { get; set; } = 1; // %
    public int Cols { get; set; } = 1;
    public bool NoWrap { get; set; } = true;
}
