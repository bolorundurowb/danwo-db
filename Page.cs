namespace DanwoDB;

public class Page
{
    public int PageNumber { get; set; }

    public byte[] Data { get; set; }

    public Page(int pageSize) => Data = new byte[pageSize];
}
