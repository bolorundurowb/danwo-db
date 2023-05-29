using DanwoDB;

await using (var dal = await DataAccessLayer.Instantiate("database.db", Environment.SystemPageSize))
{
    var page = dal.AllocateEmptyPage();
    page.PageNumber = dal.FreeList.GetNextPage();
    "data".CopyToBuffer(page.Data);

    await dal.WritePage(page);
    await dal.WriteFreeList();
}

await using (var dal = await DataAccessLayer.Instantiate("database.db", Environment.SystemPageSize))
{
    var page = dal.AllocateEmptyPage();
    page.PageNumber = dal.FreeList.GetNextPage();
    "data2".CopyToBuffer(page.Data);

    await dal.WritePage(page);

    var pageNum = dal.FreeList.GetNextPage();
    dal.FreeList.ReleasePage(pageNum);

    await dal.WriteFreeList();
}


Console.WriteLine("Hello, World!");