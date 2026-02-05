using LibGit2Sharp;
using vv.CLI.Rendering;
using vv.CLI.Settings;

namespace vv.CLI.Commands;

internal sealed class GitCommand : BaseCommand<GitSettings>
{
    private const int MAX_TOP_COUNT = 5;

    protected override int ExecuteImpl(GitSettings settings)
    {
        var repoPath = ResolvePath.ResolveRepoPath(settings);
        using var repo = new Repository(repoPath);

        var currentBranchLastCommit = repo.Head.Commits.First();
        var currentBranchFirstCommit = repo.Head.Commits.Last();

        var lastCommit = repo.Commits.First();
        var firstCommit = repo.Commits.Last();

        var contributors = repo.Commits
            .GroupBy(c => c.Author.Email)
            .Select(g => new
            {
                Name = g.First().Author.Name,
                Email = g.Key,
                CommitCount = g.Count()
            })
            .OrderByDescending(c => c.CommitCount)
            .ToList();

        string currentBranchName = repo.Head.FriendlyName;
        int currentBranchTotalCommits = repo.Head.Commits.Count();
        string currentBranchLastCommitDate = currentBranchLastCommit.Committer.When.ToString("dd.MM.yyyy HH:mm:ss");
        string currentBranchLastCommitMessage = currentBranchLastCommit.MessageShort;
        string currentBranchFirstCommitDate = currentBranchFirstCommit.Committer.When.ToString("dd.MM.yyyy HH:mm:ss");
        string currentBranchFirstCommitMessage = currentBranchFirstCommit.MessageShort;

        int totalCommits = repo.Commits.Count();
        string lastCommitMessage = lastCommit.MessageShort;
        string lastCommitDate = lastCommit.Committer.When.ToString("dd.MM.yyyy HH:mm:ss");
        string firstCommitMessage = firstCommit.MessageShort;
        string firstCommitDate = firstCommit.Committer.When.ToString("dd.MM.yyyy HH:mm:ss");

        int totalBranches = repo.Branches.Count(b => !b.IsRemote);

        int totalContributors = contributors.Count;
        var topContributors = contributors.Take(MAX_TOP_COUNT);

        DefaultRendering.Rule("[blue]Repo git stats[/]");

        var specifiedBranchGivenName = settings.BranchName;
        if (!string.IsNullOrEmpty(specifiedBranchGivenName))
        {
            if (repo.Branches[specifiedBranchGivenName] == null) return 1;

            var specifiedBranchTable = DefaultRendering.Table("Content", "Value");

            var specifiedBranch = repo.Branches[specifiedBranchGivenName];
            var specifiedBranchFirstCommit = specifiedBranch.Commits.Last();
            var specifiedBranchLastCommit = specifiedBranch.Commits.First();

            string specifiedBranchName = specifiedBranch.FriendlyName;
            int specifiedBranchTotalCommits = specifiedBranch.Commits.Count();

            string specifiedBranchLastCommitDate = specifiedBranchLastCommit.Committer.When.ToString("dd.MM.yyyy HH:mm:ss"); ;
            string specifiedBranchLastCommitMessage = specifiedBranchLastCommit.MessageShort;

            string specifiedBranchFirstCommitDate = specifiedBranchFirstCommit.Committer.When.ToString("dd.MM.yyyy HH:mm:ss"); ;
            string specifiedBranchFirstCommitMessage = specifiedBranchFirstCommit.MessageShort;

            DisplayBranch(specifiedBranchName, specifiedBranchTotalCommits, specifiedBranchFirstCommitMessage,
                specifiedBranchFirstCommitDate, specifiedBranchLastCommitMessage, specifiedBranchLastCommitDate, specifiedBranchTable);

            AnsiConsole.Write(specifiedBranchTable);
        }

        var statsTable = DefaultRendering.Table("Content", "Value");

        DisplayBranch(currentBranchName, currentBranchTotalCommits, currentBranchFirstCommitMessage,
            currentBranchFirstCommitDate, currentBranchLastCommitMessage, currentBranchLastCommitDate, statsTable);

        // Don't use DisplayBranch since it's global repo stats
        statsTable.AddEmptyRow();
        statsTable.AddRow("Commits", $"{totalCommits}");
        statsTable.AddRow("First commit message", firstCommitMessage);
        statsTable.AddRow("First commit date", firstCommitDate);
        statsTable.AddRow("Last commit message", lastCommitMessage);
        statsTable.AddRow("Last commit date", lastCommitDate);
        statsTable.AddEmptyRow();
        statsTable.AddRow("Branches", $"{totalBranches}");
        statsTable.AddEmptyRow();
        statsTable.AddRow("Contributors", $"{totalContributors}");

        foreach(var contributor in topContributors)
            statsTable.AddRow(contributor.Name, $"{contributor.CommitCount}");

        AnsiConsole.Write(statsTable);
        return 0;
    }

    private void DisplayBranch(string branchName, int branchTotalCommits, string firstCommitMessage,
        string firstCommitDate, string lastCommitMessage, string lastCommitDate, Table table,
        string displayBranchName = "Current")
    {
        table.AddRow($"{displayBranchName} branch", branchName);
        table.AddRow($"{displayBranchName} branch commits", $"{branchTotalCommits}");
        table.AddRow($"{displayBranchName} branch first commit", firstCommitMessage);
        table.AddRow($"{displayBranchName} branch first commit date", firstCommitDate);
        table.AddRow($"{displayBranchName} branch last commit", lastCommitMessage);
        table.AddRow($"{displayBranchName} branch last commit date", lastCommitDate);
    }
}