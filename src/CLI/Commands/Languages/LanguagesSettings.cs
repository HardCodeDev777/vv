using System.ComponentModel;

namespace vv.CLI.Settings;
internal class LanguagesSettings : BaseSettings
{

    [CommandOption("--ignore-extra-langs")]
    [Description("Ignore extra languages like Markdown, JSON, MSBuild, etc.")]
    public bool IgnoreDocsLangs { get; init; }

    [CommandOption("--fetch-latest")]
    [Description("Delete old \"languages.yml\" and download new from official repository")]
    public bool FetchLatest { get; init; }
}