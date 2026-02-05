using System.Diagnostics;
using System.Text.Json.Nodes;

namespace vv.Core;

internal readonly record struct LangData(string Name, long CodeCount, long CommentsCount);

internal static class Tokei
{
    private static string OutputFileName => 
        Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "langs_stats.json");

    public static async Task<bool> CheckIfTokeiInstalled()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "tokei",
            Arguments = "-v",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(psi);

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                return false;
        }
        catch
        {
            return false;
        }

        return true;
    }

    public static async Task ManageProcess(string path, bool respectIgnore)
    {
        var respectString = respectIgnore ? string.Empty : "--no-ignore";

        var psi = new ProcessStartInfo()
        {
            FileName = "cmd.exe",
            Arguments = $"/c tokei \"{path}\" {respectString} -o json > {OutputFileName} -e *.d",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

       using var process = Process.Start(psi);

        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();

        await Task.WhenAll(stdoutTask, stderrTask);
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
            throw new Exception($"Tokei process exited with error! Logs: {stderrTask.Result}");

        return;
    }

    public static List<LangData> ParseLanguageData()
    {
        var json = File.ReadAllText(OutputFileName);
        var root = JsonNode.Parse(json).AsObject();

        var languages = new List<LangData>();

        foreach (var (languageName, node) in root)
        {
            if (languageName == "Total") continue;

            var code = node["code"]?.GetValue<long>() ?? 0;
            var comments = node["comments"]?.GetValue<long>() ?? 0;

            languages.Add(new(languageName, code, comments));
        }

        return languages;
    }

    public static void DeleteGeneratedCache() => File.Delete(OutputFileName);
    
}