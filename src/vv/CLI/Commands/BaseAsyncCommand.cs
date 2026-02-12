using vv.CLI;
using vv.CLI.Rendering;

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
            DefaultRendering.WriteError(e.Message);
            return 1;
        }
        catch (Exception e)
        {
            DefaultRendering.WriteError(e.Message);
            return 2;
        }
    }

    protected abstract Task<int> ExecuteImpl(TSettings settings, CancellationToken token);
}