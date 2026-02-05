using System.Text.Json;

namespace vv.Utils;

internal static class JsonUtils
{
    public static void WriteToJson<T>(T data, string path)
    {
        string json = JsonSerializer.Serialize(data);
        File.WriteAllText(path, json);
    }

    public static TResult ReadFromJson<TResult>(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<TResult>(json); 
    }
}