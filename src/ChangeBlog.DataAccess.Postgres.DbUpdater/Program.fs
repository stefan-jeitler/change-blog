open System
open System.Data
open Microsoft.Extensions.Configuration
open Npgsql
open System.CommandLine
open System.CommandLine.Invocation
open Serilog
open Serilog.Events

let createLogger (verbose: bool) =

    let logLevel =
        if verbose then
            LogEventLevel.Verbose
        else
            LogEventLevel.Information

    LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.Console(restrictedToMinimumLevel = logLevel)
        .CreateLogger()

let verboseSwitch =
    Option<bool>([| "--verbose"; "-v" |], "Detailed output.")

let createDbUpdatesCommand (dbConnection: IDbConnection) =
    let handler (verbose: bool) =
        try
            use logger = createLogger verbose
            DbUpdatesRunner.runDbUpdates logger dbConnection
        with
        | ex ->
            printf $"%s{ex.Message}"
            -1

    let runUpdatesCommand =
        Command("run-updates", "execute all new db updates.")

    runUpdatesCommand.AddOption verboseSwitch
    runUpdatesCommand.Handler <- CommandHandler.Create<bool>(handler)

    runUpdatesCommand

let createDetectBreakingChangesCommand (dbConnection: IDbConnection) =
    let handler (verbose: bool) =
        try
            use logger = createLogger verbose
            DbUpdatesAnalysis.detectBreakingChanges logger dbConnection
        with
        | ex ->
            printf $"%s{ex.Message}"
            -1

    let description =
        "examines whether the db updates contain any breaking changes. If it does the return code is set to 1 otherwise to 0."

    let detectBreakingChangesCommand =
        Command("detect-breakingchanges", description)

    detectBreakingChangesCommand.AddOption verboseSwitch
    detectBreakingChangesCommand.Handler <- CommandHandler.Create<bool>(handler)

    detectBreakingChangesCommand

[<EntryPoint>]
let main args =

    let config =
        ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddUserSecrets("ChangeBlog.DataAccess.Postgres.DbUpdater")
            .AddEnvironmentVariables()
            .Build()

    use dbConnection =
        new NpgsqlConnection(config.GetConnectionString("ChangeBlogDb"))

    let rootCommand = RootCommand()
    rootCommand.Add(createDbUpdatesCommand dbConnection)
    rootCommand.Add(createDetectBreakingChangesCommand dbConnection)

    rootCommand.Invoke args
