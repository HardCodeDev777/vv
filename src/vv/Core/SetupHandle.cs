using LibGit2Sharp;
using vv.Utils;

namespace vv.Core;

internal readonly record struct SetupData(string RepositoriesFolder);

internal static class SetupHandle
{
    public static string SetupFilePath => 
        Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "settings.json");

    public static void WriteSetupToJson(SetupData data)
    {
        if (File.Exists(SetupFilePath)) File.Delete(SetupFilePath);

        JsonUtils.WriteToJson(data, SetupFilePath); 
    }

    public static SetupData ReadSetupFromJson()
    {
        if (!File.Exists(SetupFilePath))
            throw new Exception($"{SetupFilePath} not found!");

        return JsonUtils.ReadFromJson<SetupData>(SetupFilePath);
    }

    public static List<string> GetRepositoriesNamesFromSetup()
    {
        var repositories = new List<string>();  

        var setup = ReadSetupFromJson();

        foreach(var folder in Directory.EnumerateDirectories(setup.RepositoriesFolder))
        {
            if (Repository.IsValid(folder)) repositories.Add(Path.GetFileName(folder));
        }

        return repositories;
    }
}