using LibGit2Sharp;
using vv.Core;

namespace vv.tests;

public class SetupTest
{
    [Fact]
    public void TestGettingRepos()
    {
        var root = Directory.CreateTempSubdirectory().FullName;
        try
        {
            File.WriteAllText(Path.Combine(root, "a.cs"), "");
            Repository.Init(root);  

            var reposFolderPath = Directory.GetParent(root).FullName;
            SetupHandle.WriteSetupToJson(new(reposFolderPath));

            var reposNames = SetupHandle.GetRepositoriesNamesFromSetup();

            Assert.Contains(Path.GetFileName(root), reposNames);
        }
        finally
        {
            File.Delete(SetupHandle.SetupFilePath);
            Directory.Delete(root, true);
        }
    }
}