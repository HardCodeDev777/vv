namespace vv.Utils;

internal static class FileUtils
{
    public static string FormatBytes(this long bytes)
    {
        var sizes = new string[] { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len.ToString("0.##")} {sizes[order]}";
    }
}