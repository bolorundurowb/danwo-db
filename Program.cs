using System.Text;

var dal = new DataAccessLayer("database.db", (uint)Environment.SystemPageSize);
var page = dal.AllocateEmptyPage();
page.PageNumber = dal.FreeList.GetNextPage();

var test = "my test data";
var dataBuffer = Encoding.UTF8.GetBytes(test);
dataBuffer.CopyTo(page.Data, 0);

await dal.WritePage(page);

Console.WriteLine("Hello, World!");

class Page
{
    public uint PageNumber { get; set; }

    public byte[] Data { get; set; }

    public Page(uint pageSize) => Data = new byte[pageSize];
}

class DataAccessLayer : IAsyncDisposable
{
    public uint PageSize { get; set; }

    public FileStream File { get; set; }

    public FreeList FreeList { get; set; }

    public DataAccessLayer(string path, uint pageSize)
    {
        PageSize = pageSize;
        FreeList = new FreeList();
        File = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
    }

    public async ValueTask DisposeAsync()
    {
        await File.FlushAsync();
        File.Close();
    }

    public Page AllocateEmptyPage() => new(PageSize);

    public async Task<Page> ReadPage(uint pageNumber)
    {
        var page = AllocateEmptyPage();
        var offset = pageNumber * PageSize;
        _ = await File.ReadAsync(page.Data, (int)offset, (int)PageSize, default);

        return page;
    }

    public async Task WritePage(Page page)
    {
        var offset = page.PageNumber * PageSize;
        File.Seek(offset, SeekOrigin.Begin);
        await File.WriteAsync(page.Data, 0, (int)PageSize);
        File.Seek(0, SeekOrigin.Begin);
    }
}

class FreeList
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
