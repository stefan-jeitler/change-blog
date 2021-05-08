module SchemaTable

open System.Data
open Dapper

let private createSchemaSql = """
    CREATE TABLE IF NOT EXISTS schema_version
    (
        version INT NOT NULL,
        updated_at TIMESTAMP CONSTRAINT schemaversion_updatedat_nn NOT NULL
    )"""


let create (dbConnection: IDbConnection) =
    async {
        do!
            dbConnection.ExecuteAsync(createSchemaSql)
            |> Async.AwaitTask
            |> Async.Ignore

        do!
            dbConnection.ExecuteAsync("INSERT INTO schema_version VALUES(0, now())")
            |> Async.AwaitTask
            |> Async.Ignore
    }
