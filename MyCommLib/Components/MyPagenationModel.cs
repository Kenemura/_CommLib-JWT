namespace MyCommLib.Components;

public class MyPagenationModel
{
    public MyPagenationModel(int totalSize, int pageSize)
    {
        TotalSize = totalSize;
        PageSize = pageSize;
    }
    public int TotalSize { get; set; } = 1;
    public int TotalPages => (PageSize > 0) ? (TotalSize + PageSize - 1) / PageSize : 0;
    public int CurrentPage { get; set; } = 0;
    public int PageSize { get; set; } = 5;
    public int Prev => (CurrentPage > 1) ? CurrentPage - 1 : 1;
    public int Next => CurrentPage + 1;
}
