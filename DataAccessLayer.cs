namespace DanwoDB;

public class DataAccessLayer : IAsyncDisposable
{
    public int PageSize { get; set; }

    public FileStream File { get; set; }

    public FreeList FreeList { get; set; }

    public DataAccessLayer(string path, int pageSize)
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

    public async Task<Page> ReadPage(int pageNumber)
    {
        var page = AllocateEmptyPage();
        var offset = pageNumber * PageSize;
        _ = await File.ReadAsync(page.Data, offset, PageSize, default);

        return page;
    }

    public async Task WritePage(Page page)
    {
        var offset = page.PageNumber * PageSize;
        File.Seek(offset, SeekOrigin.Begin);
        await File.WriteAsync(page.Data, 0, PageSize);
        File.Seek(0, SeekOrigin.Begin);
    }

    public async Task WriteMeta(Meta meta)
    {
        var page = AllocateEmptyPage();
        page.PageNumber = Constants.MetadataPageNum;
        meta.Serialize().CopyTo(page.Data, 0);

        await WritePage(page);
    }

    public async Task<Meta> ReadMeta()
    {
        var page = await ReadPage(Constants.MetadataPageNum);
        var meta = Meta.Deserialize(page.Data);

        if (meta is null) 
            throw new Exception("Metadata could not be deserialized.");

        return meta;
    }
}
