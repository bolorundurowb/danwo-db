using System.Text;

namespace DanwoDB;

public static class Extensions
{
    public static void CopyToBuffer(this string input, byte[] buffer)
    {
        var dataBuffer = Encoding.UTF8.GetBytes(input);
        dataBuffer.CopyTo(buffer, 0);
    }
}
