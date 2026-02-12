using vv.CLI;
using vv.CLI.Rendering;

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
            DefaultRendering.WriteError(e.Message);
            return 1;
        }
        catch (Exception e)
        {
            DefaultRendering.WriteError(e.Message);
            return 2;
        }
    }

    protected abstract int ExecuteImpl(TSettings settings);
}