using System.ComponentModel;

namespace vv.CLI.Settings;

internal class BaseSettings : CommandSettings
{
    [CommandOption("--path")]
    [Description("Path to repository. Use when you don't need to use selection prompt")]
    public string RepoPath { get; init; }

    [CommandOption("--disrespect-gitignore")]
    [Description("Ignore all .gitignore files")]
    public bool DisrespectGitIgnore { get; init; }
}