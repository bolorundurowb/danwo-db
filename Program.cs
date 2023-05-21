// See https://aka.ms/new-console-template for more information

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

    public DataAccessLayer(string path, uint pageSize)
    {
        PageSize = pageSize;
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

    public void WritePage(Page page)
    {
        var offset = page.PageNumber * PageSize;
        var writer = new BinaryWriter(File);
        writer.Seek((int)offset, SeekOrigin.Begin);
    }
}

class FreeList
{
    public uint MaxPage { get; set; }

    public uint[] ReleasedPages { get; set; }

    public FreeList() => ReleasedPages = Array.Empty<uint>();

    public uint GetNextPage()
    {
        if (ReleasedPages.Any())
        {
            var pageNumber = ReleasedPages.Last();
            ReleasedPages = ReleasedPages.Where(x => x != pageNumber).ToArray();
            return pageNumber;
        }

        MaxPage += 1;
        return MaxPage;
    }
}
