using System.Text.Json;

namespace DanwoDB;

public class FreeList
{
    public int MaxPage { get; set; }

    public List<int> ReleasedPages { get; set; }

    public FreeList() => ReleasedPages = new List<int>();

    public int GetNextPage()
    {
        if (ReleasedPages.Any())
        {
            var pageNumber = ReleasedPages.Last();
            ReleasedPages = ReleasedPages.Where(x => x != pageNumber).ToList();
            return pageNumber;
        }

        MaxPage += 1;
        return MaxPage;
    }

    public void ReleasePage(int pageNumber) => ReleasedPages.Add(pageNumber);

    public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(this);

    public static FreeList? Deserialize(byte[] data) => JsonSerializer.Deserialize<FreeList>(data);
}
