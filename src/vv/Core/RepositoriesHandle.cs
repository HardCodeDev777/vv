using LibGit2Sharp;

namespace vv.Core;

internal readonly record struct BranchData(
    string Name, int TotalCommits, string LastCommitDate, string LastCommitMessage, 
    string FirstCommitDate, string FirstCommitMessage);

internal readonly record struct RepoData(
    IEnumerable<(string Name, string Email, int CommitsCount)> TopContributors, int TotalCommits, 
    int TotalBranches, int TotalContributors, string LastCommitDate, string FirstCommitDate);

internal static class RepositoriesHandle
{
    public static RepoData GetRepoData(Repository repo, int topContributorsCount)
    {
        var lastCommit = repo.Commits.First();
        var firstCommit = repo.Commits.Last();

        var contributors = repo.Commits
            .GroupBy(c => c.Author.Email)
            .Select(g => (
                g.First().Author.Name,
                Email: g.Key,
                CommitsCount: g.Count()
            ))
            .OrderByDescending(c => c.CommitsCount)
            .ToList();

        var totalCommits = repo.Commits.Count();
        string lastCommitDate = lastCommit.Committer.When.ToString("dd.MM.yyyy HH:mm:ss");
        string firstCommitDate = firstCommit.Committer.When.ToString("dd.MM.yyyy HH:mm:ss");

        var totalBranches = repo.Branches.Count(b => !b.IsRemote);
        var totalContributors = contributors.Count;

        var topContributors = contributors.Take(topContributorsCount);

        return new(topContributors, totalCommits, totalBranches, totalContributors, lastCommitDate, firstCommitDate);
    }


    public static BranchData GetBranchData(Branch branch)
    {
        var lastCommit = branch.Commits.First();
        var firstCommit = branch.Commits.Last();

        var branchName = branch.FriendlyName;
        var totalCommits = branch.Commits.Count();

        string lastCommitDate = lastCommit.Committer.When.ToString("dd.MM.yyyy HH:mm:ss");
        var lastCommitMessage = lastCommit.MessageShort;

        string firstCommitDate = firstCommit.Committer.When.ToString("dd.MM.yyyy HH:mm:ss");
        var firstCommitMessage = firstCommit.MessageShort;

        return new(branchName, totalCommits, lastCommitDate, lastCommitMessage, firstCommitDate, firstCommitMessage);
    }

    public static List<string> GetRepositoriesNamesFromSetup()
    {
        var repositoriesNames = new List<string>();

        var setup = SetupHandle.ReadSetupFromJson();

        foreach (var folder in Directory.EnumerateDirectories(setup.RepositoriesFolder))
        {
            if (Repository.IsValid(folder)) repositoriesNames.Add(Path.GetFileName(folder));
        }

        return repositoriesNames;
    }
}