using vv.Core;

internal abstract class BaseCommand<TSettings> : Command<TSettings> where TSettings : CommandSettings
{
    public sealed override int Execute(CommandContext context, TSettings settings, 
        CancellationToken cancellationToken)
    {
        try
        {
            return ExecuteImpl(settings);
        }
        catch (UserException e)
        {
            WriteError(e.Message);
            return 1;
        }
        catch (Exception e)
        {
            WriteError(e.Message);
            return 2;
        }
    }

    protected abstract int ExecuteImpl(TSettings settings);

    protected static void WriteError(Exception exception) =>
        AnsiConsole.WriteException(exception);

    protected static void WriteError(string message) =>
        AnsiConsole.MarkupLine($"[red]Error: {Markup.Escape(message)}[/]");

    protected static void WriteSuccess(string message) => AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
}