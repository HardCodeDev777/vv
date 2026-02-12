using System.ComponentModel;

namespace vv.CLI.Settings;

internal class GitSettings : BaseSettings 
{
    [CommandOption("--branch")]
    [Description("Also display specified branch info")]
    public string BranchName { get; init; }
}