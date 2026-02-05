using LibGit2Sharp;

namespace vv.Core;

internal static class AdvancedEnumerate
{
    public static void Walk(string root, Repository repo, bool respectGitIgnore, Action<string> onEntryDir, Action<string> onExitDir, Action<string> onFile)
    {
        onEntryDir(root);

        foreach (var dir in Directory.EnumerateDirectories(root))
        {
            if (respectGitIgnore)
            {
                var rel = Path.GetRelativePath(repo.Info.WorkingDirectory, dir)
                    .Replace('\\', '/') + "/";

                if (repo.Ignore.IsPathIgnored(rel)) continue;
            }

            Walk(dir, repo, respectGitIgnore, onEntryDir, onExitDir, onFile);
        }

        foreach (var file in Directory.EnumerateFiles(root))
        {
            if (respectGitIgnore)
            {
                var rel = Path.GetRelativePath(repo.Info.WorkingDirectory, file).Replace('\\', '/');

                if (repo.Ignore.IsPathIgnored(rel)) continue;
            }

            onFile(file);
        }

        onExitDir(root);
    }

}