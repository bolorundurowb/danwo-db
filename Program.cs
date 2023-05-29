using System.Text;
using danwo_db;

var dal = new DataAccessLayer("database.db", (uint)Environment.SystemPageSize);
var page = dal.AllocateEmptyPage();
page.PageNumber = dal.FreeList.GetNextPage();

var test = "my test data";
var dataBuffer = Encoding.UTF8.GetBytes(test);
dataBuffer.CopyTo(page.Data, 0);

await dal.WritePage(page);

Console.WriteLine("Hello, World!");
