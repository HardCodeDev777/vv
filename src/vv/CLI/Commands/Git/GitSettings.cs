using System.ComponentModel;

namespace vv.CLI.Settings;

internal class GitSettings : BaseSettings 
{
    [CommandOption("--branch")]
    [Description("Display basic repository and specified branch info")]
    public string BranchName { get; init; }
}