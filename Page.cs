namespace danwo_db;

public class Page
{
    public uint PageNumber { get; set; }

    public byte[] Data { get; set; }

    public Page(uint pageSize) => Data = new byte[pageSize];
}
