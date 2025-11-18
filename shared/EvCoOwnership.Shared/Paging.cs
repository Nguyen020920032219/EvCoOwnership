namespace EvCoOwnership.Shared;

public class PagingRequest
{
    public int PageIndex { get; set; } = 1;
    public int PageSize  { get; set; } = 20;
}

public class PagingResponse<T>
{
    public int TotalItems { get; set; }
    public int PageIndex  { get; set; }
    public int PageSize   { get; set; }
    public IList<T> Items { get; set; } = new List<T>();
}