using vv.CLI.Settings;
using vv.Core;

namespace vv.CLI.Commands;

internal class SetupCommand : BaseAsyncCommand<SetupSettings>
{
    protected override async Task<int> ExecuteImpl(SetupSettings settings, CancellationToken cancellationToken)
    {
        // Currently only one dependency
        if (AnsiConsole.Confirm("Do you want to check required dependencies?"))
        {
            var installed = await Tokei.CheckIfTokeiInstalled();
            if (installed)
                AnsiConsole.MarkupLine("[green]All required dependencies are satisfied![/]");
            else
                WriteError("Install tokei and add it to PATH");
        }

        if (AnsiConsole.Confirm("Do you want to locate folder for repositories?"))
        {
            var path = AnsiConsole.Ask<string>("[bold blue]Absolute path to repositories folder: [/]")
                .Replace("\"", "");

            if (string.IsNullOrEmpty(path))
                WriteError("Path cannot be empty");
            
            SetupHandle.WriteSetupToJson(new(path));

            AnsiConsole.MarkupLine($"[dim white]Settings were written to {SetupHandle.SetupFilePath}![/]");
        }

        return 0;
    }
}
