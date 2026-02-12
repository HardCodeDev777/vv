using System.Diagnostics;
using System.Text.Json.Nodes;

namespace vv.Core;

internal readonly record struct LangData(string Name, long CodeCount, long CommentsCount);

internal static class Tokei
{

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

    public static async Task<string> ManageProcess(string path, bool respectIgnore)
    {
        var respectString = respectIgnore ? string.Empty : "--no-ignore";

        var psi = new ProcessStartInfo
        {
            FileName = "tokei",
            Arguments = $"\"{path}\" {respectString} -o json -e *.d",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
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

        return stdoutTask.Result;
    }

    public static List<LangData> ParseLanguageData(string tokeiOutput)
    {
        var root = JsonNode.Parse(tokeiOutput).AsObject();

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
}