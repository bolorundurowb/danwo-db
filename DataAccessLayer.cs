namespace danwo_db;

public class DataAccessLayer : IAsyncDisposable
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
