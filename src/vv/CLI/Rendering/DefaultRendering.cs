namespace vv.CLI.Rendering;

internal static class DefaultRendering
{
    public static void Progress(Action<ProgressContext> ctx)
    {
        AnsiConsole.Progress()
           .AutoClear(true)
           .Columns(
               new SpinnerColumn()
               {
                   Spinner = Spectre.Console.Spinner.Known.Dots,
                   Style = Style.Parse("bold blue")
               },
               new TaskDescriptionColumn(),
               new ProgressBarColumn()
               {
                   CompletedStyle = new Style(Color.Green),
                   FinishedStyle = new Style(Color.Lime),
                   RemainingStyle = new Style(Color.Grey)
               },
               new ElapsedTimeColumn())
           .Start(ctx);
    }

    public static async Task Spinner(string status, Func<StatusContext, Task> action)
    {
        await AnsiConsole.Status()
           .Spinner(Spectre.Console.Spinner.Known.Dots)
           .SpinnerStyle(Style.Parse("bold blue"))
           .StartAsync(status, action);
    }

    public static void Rule(string title)
    {
        AnsiConsole.Write(new Rule(title)
            .RuleStyle(new Style(Color.Grey)));
    }

    public static TResult Prompt<TResult>(string title, IEnumerable<TResult> options)
    {
        return AnsiConsole.Prompt(new SelectionPrompt<TResult>()
            .Title(title)
            .EnableSearch()
            .WrapAround()
            .AddChoices(options));
    }

    public static Table Table(params string[] columns) 
    {
        var resultTable = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Blue);

        foreach (var column in columns )
            resultTable.AddColumn(column);
        
        return resultTable;
    }
}