using vv.Core;

internal abstract class BaseAsyncCommand<TSettings> : AsyncCommand<TSettings> where TSettings : CommandSettings
{
    public sealed override async Task<int> ExecuteAsync(CommandContext context, TSettings settings,
        CancellationToken cancellationToken)
    {
        try
        {
            return await ExecuteImpl(settings, cancellationToken); ;
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

    protected abstract Task<int> ExecuteImpl(TSettings settings, CancellationToken token);

    protected static void WriteError(Exception exception) =>
        AnsiConsole.WriteException(exception);

    protected static void WriteError(string message) =>
        AnsiConsole.MarkupLine($"[red]Error: {Markup.Escape(message)}[/]");

    protected static void WriteSuccess(string message) => AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
}