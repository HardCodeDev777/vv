using LibGit2Sharp;
using vv.Utils;

namespace vv.Core;

internal readonly record struct SetupData(string RepositoriesFolder);

internal static class SetupHandle
{
    public static string SetupFileName => 
        Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "settings.json");

    public static void WriteSetupToJson(SetupData data)
    {
        if (File.Exists(SetupFileName)) File.Delete(SetupFileName);

        JsonUtils.WriteToJson(data, SetupFileName); 
    }

    public static SetupData ReadSetupFromJson()
    {
        if (!File.Exists(SetupFileName))
            throw new Exception($"{SetupFileName} not found!");

        return JsonUtils.ReadFromJson<SetupData>(SetupFileName);
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