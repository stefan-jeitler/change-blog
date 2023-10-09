module DbUpdatesRunner

open DbUpdates
open Serilog.Core

let private executeUpdate (logger: Logger) dbConnection dbUpdate =
    let version = dbUpdate.Version
    logger.Information("Update database to Version {version}", version.ToString())
    dbUpdate.Update dbConnection
    Db.updateSchemaVersion dbConnection version

let private findDuplicates (updates: DbUpdate list) =

    updates
    |> List.groupBy (fun x -> x.Version)
    |> List.choose (function
        | v, u when u.Length > 1 -> Some v
        | _ -> None)

let private runUniqueUpdates (logger: Logger) dbConnection dbUpdates =
    let latestSchemaVersion = Db.getLatestSchemaVersion dbConnection
    let dbName = Db.getDbName dbConnection

    logger.Information("Selected database: {dbName}", dbName)
    logger.Verbose("Latest schema version: {version}", latestSchemaVersion)

    let runUpdate = executeUpdate logger dbConnection

    let executedUpdates =
        dbUpdates
        |> Seq.skipWhile (fun x -> x.Version <= latestSchemaVersion)
        |> Seq.map runUpdate
        |> Seq.toList

    match executedUpdates with
    | [] -> logger.Information("No database updates to execute.")
    | u ->
        logger.Verbose("{count} db update(s) executed.", u.Length)
        logger.Verbose("Latest schema version: {version}", (Db.getLatestSchemaVersion dbConnection))

    ()

let runDbUpdates (logger: Logger) dbConnection =
    let duplicates = findDuplicates dbUpdates |> List.map string

    match duplicates with
    | [] ->
        runUniqueUpdates logger dbConnection dbUpdates
        0
    | d ->
        logger.Error("There are duplicate updates. Version(s) {duplicates}", (d |> String.concat ", "))
        -1
