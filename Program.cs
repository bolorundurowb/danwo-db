// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");

class Page
{
    public uint PageNumber { get; set; }

    public byte[] Data { get; set; }
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

    public Page AllocateEmptyPage()
    {
        
    }
}