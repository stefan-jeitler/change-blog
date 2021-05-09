open System
open Microsoft.Extensions.Configuration
open Npgsql
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

        dbUpdates
        |> Seq.skipWhile (fun x -> x.Version <= latestSchemaVersion)
        |> Seq.map (executeUpdate dbConnection)
        |> Async.Sequential
        |> Async.Ignore
        |> Async.RunSynchronously
    }

let findDuplicates (u: DbUpdate list) =
    u
    |> List.groupBy (fun x -> x.Version)
    |> List.choose
        (function
        | v, u when u.Length > 1 -> Some v
        | _ -> None)

[<EntryPoint>]
let main _ =

    let duplicates =
        findDuplicates dbUpdates |> List.map string

    match duplicates with
    | [] -> ()
    | d -> failwith (sprintf "Duplicate updates exists. Version(s) %s" (d |> String.concat ", "))

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
