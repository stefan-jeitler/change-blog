module DbUpdatesAnalysis

open System.Data
open Serilog.Core

let detectBreakingChanges (logger: Logger) (dbConnection: IDbConnection) =
    let dbName = Db.getDbName dbConnection
    let latestDbVersion = Db.getLatestSchemaVersion dbConnection

    let latestDbUpdatesVersion =
        DbUpdates.dbUpdates
        |> List.map (fun x -> x.Version)
        |> List.max

    logger.Information("Selected database: {dbName}", dbName)
    logger.Verbose("Latest db version: {dbVersion}", latestDbVersion.ToString())
    logger.Verbose("Latest update version: {updatesVersion}", latestDbUpdatesVersion.ToString())

    if latestDbVersion.Major <> latestDbUpdatesVersion.Major then
        logger.Information("Breaking changes detected.")
        1
    else
        logger.Information("There are no breaking changes.")
        0
