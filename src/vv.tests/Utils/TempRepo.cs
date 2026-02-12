using LibGit2Sharp;

namespace vv.tests.Utils;

internal static class TempRepo
{
    public static Repository CreateTempRepository(out string repoPath)
    {
        var templateRepoPath = Path.GetFullPath(Path.Combine(
                new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.FullName, "TestRepository"));

        var tempRepoPath = Directory.CreateTempSubdirectory().FullName;
        Console.WriteLine($"Temp repository path: {tempRepoPath}");

        foreach(var filePath in Directory.GetFiles(templateRepoPath))
        {
            var fileName = Path.GetFileName(filePath);  
            File.Copy(Path.Combine(templateRepoPath, fileName), Path.Combine(tempRepoPath, fileName));
        }

        File.Copy(Path.Combine(templateRepoPath, "gitignore.txt"), Path.Combine(tempRepoPath, ".gitignore"));

        Repository.Init(tempRepoPath);
        using var repo = new Repository(tempRepoPath);
       
        repoPath = tempRepoPath;
        return repo;
    }

    public static void DeleteTempRepository(string repoPath)
    {
        var directory = new DirectoryInfo(repoPath);

        foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
            file.Attributes = FileAttributes.Normal;
        
        foreach (var dir in directory.GetDirectories("*", SearchOption.AllDirectories))
            dir.Attributes = FileAttributes.Normal;  

        directory.Delete(true);
    }
}
