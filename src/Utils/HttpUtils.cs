namespace vv.Utils;

internal static class HttpUtils
{
    private static readonly HttpClient Client = new();

    public static async Task<byte[]> DownloadFileBytes(string url)
    {
        var fileBytes = await Client.GetByteArrayAsync(url);
        return fileBytes;
    }
}