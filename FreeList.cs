namespace danwo_db;

public class FreeList
{
    public uint MaxPage { get; set; }

    public List<uint> ReleasedPages { get; set; }

    public FreeList() => ReleasedPages = new List<uint>();

    public uint GetNextPage()
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

    public void ReleasePage(uint pageNumber) => ReleasedPages.Add(pageNumber);
}
