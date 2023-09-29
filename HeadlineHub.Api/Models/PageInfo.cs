namespace HeadlineHub.Api.Models;

public class PageInfo
{
    public int? Index { get; set; }
    
    public int? Size { get; set; }

    public PageInfo(int? index, int? size)
    {
        Index = index;
        Size = size;
    }
}