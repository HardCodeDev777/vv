using LibGit2Sharp;
using vv.CLI.Rendering;
using vv.CLI.Settings;
using vv.Core;

namespace vv.CLI.Commands;

internal sealed class TreeCommand : BaseCommand<TreeSettings>
{
    protected override int ExecuteImpl(TreeSettings settings)
    {
        var repoPath = ResolvePath.ResolveRepoPath(settings);
        using var repo = new Repository(repoPath);

        long processed = 0;

        var tree = new Spectre.Console.Tree("[bold blue]Repo Tree[/]")
            .Guide(TreeGuide.DoubleLine)
            .Style(Style.Parse("blue bold"));

        var repoName = Path.GetFileName(settings.RepoPath);
        var rootNode = tree.AddNode($"[green]{repoName}/[/]");

        Stack<TreeNode> nodes = new();
        nodes.Push(rootNode);

        DefaultRendering.Progress(ctx =>
        {
            var task = ctx.AddTask("Scanning repository");
            task.IsIndeterminate = true;

            AdvancedEnumerate.Walk(repoPath, repo, !settings.DisrespectGitIgnore,
            entryDir =>
            {
                var name = Path.GetFileName(entryDir);

                if (repoPath == entryDir) return;
                var node = nodes.Peek().AddNode($"[green]{name}/[/]");
                nodes.Push(node);
            },
            exitDir =>
            {
                nodes.Pop();

                processed++;
                task.Description = $"[bold blue]Processing {processed}[/]";
            },
            entryFile =>
            {
                var name = Path.GetFileName(entryFile);
                nodes.Peek().AddNode(name);

                processed++;
                task.Description = $"[bold blue]Processing {processed}[/]";
            });
        });

        AnsiConsole.WriteLine();
        AnsiConsole.Write(tree);
        return 0;
    }
}
