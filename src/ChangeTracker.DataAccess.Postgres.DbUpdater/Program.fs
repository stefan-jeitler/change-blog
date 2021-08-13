open System
open System.Data
open Microsoft.Extensions.Configuration
open Npgsql
open System.CommandLine
open System.CommandLine.Invocation
open Serilog
open Serilog.Events

let verboseSwitch = Option<bool>([|"--verbose"; "-v"|], "Detailed output.")

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

let createDbUpdatesCommand (dbConnection: IDbConnection) =
    let runUpdatesCommand =
        Command("run-updates", "executes all new db updates")

    let handler (verbose: bool) =
        try
            use logger = createLogger verbose
            DbUpdatesRunner.runDbUpdates logger dbConnection
        with ex ->
            printf "%s" ex.Message
            -1

    runUpdatesCommand.AddOption verboseSwitch
    runUpdatesCommand.Handler <- CommandHandler.Create<bool>((fun (verbose) -> handler verbose))

    runUpdatesCommand

let createDetectBreakingChangesCommand (dbConnection: IDbConnection) =
    let description =
        "examines whether the db updates contain any breaking changes. If it does the return code is set to 1 otherwise to 0."

    let containsBreakingChangesCommand =
        Command("detect-breakingchanges", description)

    let handler (verbose: bool) =
      try 
          use logger = createLogger verbose
          DbUpdatesAnalysis.detectBreakingChanges logger dbConnection
      with ex ->
          printf "%s" ex.Message
          -1

    containsBreakingChangesCommand.AddOption verboseSwitch
    containsBreakingChangesCommand.Handler <- CommandHandler.Create<bool>((fun (verbose) -> handler verbose))

    containsBreakingChangesCommand

[<EntryPoint>]
let main args =

    let config =
        ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddUserSecrets("ChangeTracker.DataAccess.Postgres.DbUpdater")
            .AddEnvironmentVariables()
            .Build()

    use dbConnection =
        new NpgsqlConnection(config.GetConnectionString("ChangeTrackerDb"))

    let rootCommand = RootCommand()
    rootCommand.Add(createDbUpdatesCommand dbConnection)
    rootCommand.Add(createDetectBreakingChangesCommand dbConnection)

    rootCommand.Invoke args
