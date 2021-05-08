open System
open Microsoft.Extensions.Configuration
open Npgsql

open DbUpdater
open DbUpdates

let executeUpdate dbConnection dbUpdate =
    async {
        let version = dbUpdate.Version
        printf "Udpate database to Version %i\n" version
        do! dbUpdate.Update dbConnection
        do! Db.updateSchemaVersion dbConnection version
    }

let runDbUpdates dbConnection =
    async {
        let! latestSchemaVersion = Db.getLatestSchemaVersion dbConnection
        let! dbName = Db.getDbName dbConnection
        printf "Selected database: %s\n" dbName

        return
            dbUpdates
            |> Seq.skipWhile (fun x -> x.Version <= latestSchemaVersion)
            |> Seq.map (executeUpdate dbConnection)
            |> Async.Sequential
            |> Async.RunSynchronously
    }

[<EntryPoint>]
let main _ =
    let config =
        (new ConfigurationBuilder())
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddUserSecrets("ChangeTracker.DataAccess.Postgres.DbUpdater")
            .AddEnvironmentVariables()
            .Build()

    use dbConnection =
        new NpgsqlConnection(config.GetConnectionString("ChangeTrackerDb"))

    printf "Run db updates ...\n"

    runDbUpdates dbConnection
    |> Async.Ignore
    |> Async.RunSynchronously

    0
