using vv.CLI.Rendering;
using vv.CLI.Settings;
using vv.Core;

namespace vv.CLI.Commands;

internal sealed class LanguagesCommand : BaseAsyncCommand<LanguagesSettings>
{
    protected override async Task<int> ExecuteImpl(LanguagesSettings settings, CancellationToken cancellationToken)
    {
        var repoPath = ResolvePath.ResolveRepoPath(settings);
        var fullLangsData = new Dictionary<LangData, string>();
       
        await DefaultRendering.Spinner("Starting processing...", async ctx =>
        {
            if (settings.FetchLatest) 
            {
                ctx.Status("Updating \"languages.yml\"...");
                await LanguagesYml.UpdateLanguagesYmlFile(); 
            }

            ctx.Status("Running tokei...");
            var jsonOutput = await Tokei.ManageProcess(repoPath, !settings.DisrespectGitIgnore);

            ctx.Status("Parsing tokei output...");
            var langsData = Tokei.ParseLanguageData(jsonOutput);

            ctx.Status("Getting founded languages colors from languages.yml ...");
            fullLangsData = LanguagesYml.GetLangsColors(langsData, settings.IgnoreExtraLangs);
        });

        AnsiConsole.WriteLine();

        var langsCodeChart = new BarChart()
            .Label("[bold blue]Code stats[/]");
        var langsCommentsChart = new BarChart()
            .Label("[bold blue]Comments stats[/]");
        var langsTotalChart = new BarChart()
            .Label("[bold blue]Total stats[/]");

        foreach(var data in fullLangsData)
        {
            langsCodeChart.AddItem(data.Key.Name, data.Key.CodeCount, Color.FromHex(data.Value));   
            langsCommentsChart.AddItem(data.Key.Name, data.Key.CommentsCount, Color.FromHex(data.Value));   
            langsTotalChart.AddItem(data.Key.Name, (data.Key.CommentsCount + data.Key.CodeCount), Color.FromHex(data.Value));   
        }

        AnsiConsole.Write(langsCodeChart);
        AnsiConsole.WriteLine();
        AnsiConsole.Write(langsCommentsChart);
        AnsiConsole.WriteLine();
        AnsiConsole.Write(langsTotalChart);

        return 0;
    }
}
