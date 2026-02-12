using LibGit2Sharp;
using vv.CLI.Settings;
using vv.Core;

namespace vv.CLI.Rendering;

internal static class ResolvePath
{
    public static string ResolveRepoPath<T>(T settings) where T : BaseSettings
    {
        var repoPath = settings.RepoPath;
        if (string.IsNullOrEmpty(repoPath))
        {
            var reposNames = RepositoriesHandle.GetRepositoriesNamesFromSetup();

            if (reposNames.Count == 0)
                throw new UserException(
                    "To select repository from prompt you need to configure default folder for repositories. Run \"vv setup\"");

            var repoName = DefaultRendering.Prompt("Select repository", reposNames);
            var reposFolderName = SetupHandle.ReadSetupFromJson().RepositoriesFolder;
            repoPath = Path.Combine(reposFolderName, repoName);

            if (string.IsNullOrEmpty(repoPath) || !Repository.IsValid(repoPath))
                throw new UserException("Couldn't locate repository!");
        }

        return repoPath;
    }
}