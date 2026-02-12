using LibGit2Sharp;
using vv.CLI.Rendering;
using vv.CLI.Settings;
using vv.Core;

namespace vv.CLI.Commands;

internal sealed class GitCommand : BaseCommand<GitSettings>
{
    private const int MAX_TOP_COUNT = 5;

    protected override int ExecuteImpl(GitSettings settings)
    {
        var repoPath = ResolvePath.ResolveRepoPath(settings);
        using var repo = new Repository(repoPath);

        var repoData = RepositoriesHandle.GetRepoData(repo, MAX_TOP_COUNT);
        var currentBranchData = RepositoriesHandle.GetBranchData(repo.Head);

        DefaultRendering.Rule("[blue]Repo git stats[/]");

        var specifiedBranchGivenName = settings.BranchName;
        if (!string.IsNullOrEmpty(specifiedBranchGivenName))
        {
            if (repo.Branches[specifiedBranchGivenName] == null) return 1;

            var specifiedBranchTable = DefaultRendering.Table("Content", "Value");

            var specifiedBranchData = RepositoriesHandle.GetBranchData(repo.Branches[specifiedBranchGivenName]);

            DisplayBranch(specifiedBranchData.Name, specifiedBranchData.TotalCommits,
                specifiedBranchData.FirstCommitMessage, specifiedBranchData.FirstCommitDate,
                specifiedBranchData.LastCommitMessage, specifiedBranchData.LastCommitDate,
               specifiedBranchTable);

            AnsiConsole.Write(specifiedBranchTable);
        }

        var statsTable = DefaultRendering.Table("Content", "Value");

        DisplayBranch(currentBranchData.Name, currentBranchData.TotalCommits, 
            currentBranchData.FirstCommitMessage, currentBranchData.FirstCommitDate,
            currentBranchData.LastCommitMessage, currentBranchData.LastCommitDate,
             statsTable);

        // Don't use DisplayBranch since it's global repo stats
        statsTable.AddEmptyRow();
        statsTable.AddRow("Commits", $"{repoData.TotalCommits}");
        statsTable.AddRow("First commit date", repoData.FirstCommitDate);
        statsTable.AddRow("Last commit date", repoData.LastCommitDate);
        statsTable.AddEmptyRow();
        statsTable.AddRow("Branches", $"{repoData.TotalBranches}");
        statsTable.AddEmptyRow();
        statsTable.AddRow("Contributors", $"{repoData.TotalCommits}");

        foreach(var contributor in repoData.TopContributors)
            statsTable.AddRow(contributor.Name, $"{contributor.CommitsCount}");

        AnsiConsole.Write(statsTable);
        return 0;
    }

    private void DisplayBranch(string branchName, int branchTotalCommits, string firstCommitMessage,
        string firstCommitDate, string lastCommitMessage, string lastCommitDate, Table table)
    {
        var displayBranchName = "Current";

        table.AddRow($"{displayBranchName} branch", branchName);
        table.AddRow($"{displayBranchName} branch commits", $"{branchTotalCommits}");
        table.AddRow($"{displayBranchName} branch first commit", firstCommitMessage);
        table.AddRow($"{displayBranchName} branch first commit date", firstCommitDate);
        table.AddRow($"{displayBranchName} branch last commit", lastCommitMessage);
        table.AddRow($"{displayBranchName} branch last commit date", lastCommitDate);
    }
}