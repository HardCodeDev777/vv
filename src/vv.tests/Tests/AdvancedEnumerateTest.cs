using LibGit2Sharp;
using vv.Core;

namespace vv.tests;

public class AdvancedEnumerateTest
{
    [Fact]
    public void TestEnumerate()
    {
        var root = Directory.CreateTempSubdirectory().FullName;

        try
        {
            Directory.CreateDirectory(Path.Combine(root, "src"));
            Directory.CreateDirectory(Path.Combine(root, "bin"));

            File.WriteAllText(Path.Combine(root, "src", "a.cs"), "");
            File.WriteAllText(Path.Combine(root, "bin", "b.exe"), "");

            File.WriteAllText(Path.Combine(root, ".gitignore"), "bin/\n");

            Repository.Init(root);
            using var repo = new Repository(root);

            var checkedDirs = new List<string>();

            AdvancedEnumerate.Walk(root, repo, true,
                onEntryDir: d => checkedDirs.Add(Path.GetFileName(d)),
                _ => { }, _ => { }
            );

            Assert.Contains("src", checkedDirs);
            Assert.DoesNotContain("bin", checkedDirs);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }
}
