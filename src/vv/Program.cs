using vv.CLI.Commands;

Console.OutputEncoding = System.Text.Encoding.UTF8;
LibGit2Sharp.GlobalSettings.SetOwnerValidation(false);

var app = new CommandApp();
app.Configure(config =>
{
    config.SetApplicationCulture(new("en-US"));
    config.SetApplicationName("vv");
    config.SetApplicationVersion("0.1.0");

    config.AddCommand<SetupCommand>("setup")
        .WithDescription("Setup vv before usage")
        .WithExample("setup");

    config.AddCommand<FsCommand>("fs")
        .WithDescription("Display basic repository filesystem info such as files count, files size, etc.")
        .WithExample("fs")
        .WithExample("fs", "--path", "path/to/repository");

    config.AddCommand<LanguagesCommand>("langs")
        .WithDescription("Display detailed repository languages statistics")
        .WithExample("langs")
        .WithExample("langs", "--path", "path/to/repository")
        .WithExample("langs", "--ignore-extra-langs")
        .WithExample("langs", "--fetch-latest");

    config.AddCommand<TreeCommand>("tree")
        .WithDescription("Shows repository filesystem tree")
        .WithExample("tree")
        .WithExample("tree", "--path", "path/to/repository");

    config.AddCommand<GitCommand>("git")
        .WithDescription("Shows repository git info")
        .WithExample("git")
        .WithExample("git", "--path", "path/to/repository")
        .WithExample("git", "--branch", "branchname");
});
app.Run(args);