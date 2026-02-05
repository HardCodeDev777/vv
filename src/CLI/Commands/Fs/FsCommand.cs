using LibGit2Sharp;
using vv.CLI.Rendering;
using vv.CLI.Settings;
using vv.Core;
using vv.Utils;

namespace vv.CLI.Commands;

internal sealed class FsCommand : BaseCommand<FsSettings>
{
    private const int MAX_TOP_COUNT = 3;

    protected override int ExecuteImpl(FsSettings settings)
    {
        var repoPath = ResolvePath.ResolveRepoPath(settings);
        using var repo = new Repository(repoPath);

        long processedDirs = 0, processedFiles = 0, filesCount = 0, dirsCount = 0, filesSize = 0;

        Dictionary<string, long> topFiles = new(), topDirs = new();
        Stack<long> filesSizesInCurrentDir = new();

        DefaultRendering.Progress(ctx =>
        {
            var task = ctx.AddTask("Scanning repository");
            task.IsIndeterminate = true;

            AdvancedEnumerate.Walk(repoPath, repo, !settings.DisrespectGitIgnore, _ => 
            {
                filesSizesInCurrentDir.Push(0);
            }, 
            dir => 
            {
                if (dir == repoPath) return;

                processedDirs++;
                task.Description = $"[bold blue]Processing {processedDirs + processedFiles} [/]";
                dirsCount++;

                var dirName = Path.GetFileName(dir);
                var dirSize = filesSizesInCurrentDir.Pop();
                if (filesSizesInCurrentDir.Count > 0)
                {
                    var parent = filesSizesInCurrentDir.Pop();
                    filesSizesInCurrentDir.Push(parent + dirSize);
                }

                if (topDirs.Count == 0)
                {
                    topDirs.Add(dirName, dirSize);
                    return;
                }

                CheckTop(topDirs, dirSize, dirName);
            }, 
            entryFile =>
            {
                processedFiles++;
                task.Description = $"[bold blue]Processing {processedDirs + processedFiles} [/]";
                filesCount++;

                var fileSize = new FileInfo(entryFile).Length;
                var fileName = Path.GetFileName(entryFile);

                filesSize += fileSize;

                var current = filesSizesInCurrentDir.Pop();
                filesSizesInCurrentDir.Push(current + fileSize);

                if (topFiles.Count == 0) 
                { 
                    topFiles.Add(fileName, fileSize);
                    return;
                }

                CheckTop(topFiles, fileSize, fileName);
            });
        });
        AnsiConsole.WriteLine();

        DefaultRendering.Rule("[blue]Repo filesystem stats[/]");

        var statsTable = DefaultRendering.Table("Content", "Value");;

        statsTable.AddRow("Files", $"{filesCount}");
        statsTable.AddRow("Directories", $"{dirsCount}");
        statsTable.AddRow("Size", filesSize.FormatBytes());
        statsTable.AddRow("Avg file size", (filesSize / processedFiles).FormatBytes());

        var topFilesTable = DefaultRendering.Table("File Name", "Size");

        foreach (var fileData in topFiles.OrderByDescending(x => x.Value))
            topFilesTable.AddRow(fileData.Key, $"{fileData.Value.FormatBytes()}");   
        
        var topDirsTable = DefaultRendering.Table("Directory Name", "Size");

        foreach (var dirData in topDirs.OrderByDescending(x => x.Value))
            topDirsTable.AddRow(dirData.Key, $"{dirData.Value.FormatBytes()}");  

        AnsiConsole.Write(statsTable);
        AnsiConsole.Write(topFilesTable);
        AnsiConsole.Write(topDirsTable);

        return 0;
    }

    private void CheckTop(Dictionary<string, long> top, long size, string name)
    {
        if (top.ContainsKey(name)) return;

        if (top.Count < MAX_TOP_COUNT)
        {
            top.Add(name, size);
            return;
        }

        var smallest = top.MinBy(x => x.Value);

        if (size > smallest.Value)
        {
            top.Remove(smallest.Key);
            top.Add(name, size);
        }
    }

}
