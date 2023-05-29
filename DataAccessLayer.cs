namespace DanwoDB;

public class DataAccessLayer : IAsyncDisposable
{
    public int PageSize { get; set; }

    public FileStream File { get; set; }

    public FreeList FreeList { get; set; }

    public Meta Metadata { get; set; }

    private DataAccessLayer(string path, int pageSize)
    {
        PageSize = pageSize;
        File = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

        FreeList = new FreeList();
        Metadata = new Meta
        {
            FreeListPage = FreeList.GetNextPage()
        };
    }

    public static async Task<DataAccessLayer> Instantiate(string path, int pageSize)
    {
        var isExistingDb = System.IO.File.Exists(path);
        var dal = new DataAccessLayer(path, pageSize);
        

        if (isExistingDb)
        {
            await dal.ReadMeta();
            await dal.ReadFreeList();
        }
        else
        {
            await dal.WriteFreeList();
            await dal.WriteMeta();
        }

        return dal;
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
        File.Seek(offset, SeekOrigin.Begin);
        _ = await File.ReadAsync(page.Data, 0, PageSize, default);
        File.Seek(0, SeekOrigin.Begin);

        return page;
    }

    public async Task WritePage(Page page)
    {
        var offset = page.PageNumber * PageSize;
        File.Seek(offset, SeekOrigin.Begin);
        await File.WriteAsync(page.Data, 0, PageSize);
        File.Seek(0, SeekOrigin.Begin);
    }

    public async Task WriteMeta()
    {
        var page = AllocateEmptyPage();
        page.PageNumber = Constants.MetadataPageNum;
        Metadata.Serialize().CopyTo(page.Data, 0);

        await WritePage(page);
    }

    public async Task ReadMeta()
    {
        var page = await ReadPage(Constants.MetadataPageNum);
        var meta = Meta.Deserialize(page.Data);
        Metadata = meta ?? throw new Exception("Metadata could not be deserialized.");
    }

    public async Task WriteFreeList()
    {
        var page = AllocateEmptyPage();
        page.PageNumber = Metadata.FreeListPage;
        FreeList.Serialize().CopyTo(page.Data, 0);

        await WritePage(page);
    }

    public async Task ReadFreeList()
    {
        var page = await ReadPage(Metadata.FreeListPage);
        var freeList = FreeList.Deserialize(page.Data);
        FreeList = freeList ?? throw new Exception("Freelist could not be deserialized.");
    }
}
