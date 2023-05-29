using System.Text.Json;

namespace DanwoDB;

public class Meta
{
    public int FreeListPage { get; set; }

    public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(this);

    public static Meta? Deserialize(byte[] data) => JsonSerializer.Deserialize<Meta>(data.TakeWhile(x => x != default).ToArray());
}
