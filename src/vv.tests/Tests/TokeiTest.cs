using vv.tests.Utils;
using vv.Core;

namespace vv.tests;

public class TokeiTest
{
    [Fact]
    public async Task TestWithIgnore()
    {
        var repoPath = string.Empty;
        try
        {
            var testRepo = TempRepo.CreateTempRepository(out repoPath);

            var outputJson = await Tokei.ManageProcess(repoPath, true);
            var data = Tokei.ParseLanguageData(outputJson);

            Assert.NotNull(data);
            Assert.NotEmpty(data);

            var langsNames = new List<string>();
            foreach (var langData in data)
                langsNames.Add(langData.Name);

            Assert.Contains("C#", langsNames);
            Assert.DoesNotContain("Batch", langsNames);
        }
        finally
        {

            if (!string.IsNullOrEmpty(repoPath)) 
                TempRepo.DeleteTempRepository(repoPath);
        }
    }

    [Fact]
    public async Task TestWithoutIgnore()
    {
        var repoPath = string.Empty;
        try
        {
            var testRepo = TempRepo.CreateTempRepository(out repoPath);

            var outputJson = await Tokei.ManageProcess(repoPath, false);
            var data = Tokei.ParseLanguageData(outputJson);

            var langsNames = new List<string>();
            foreach (var langData in data)
                langsNames.Add(langData.Name);

            Assert.Contains("Batch", langsNames);
        }

        finally
        {
            if (!string.IsNullOrEmpty(repoPath))
                TempRepo.DeleteTempRepository(repoPath);
        }
    }
}