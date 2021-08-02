module SchemaTable

open System.Data
open Dapper

let private createSchemaSql =
    """
    CREATE TABLE IF NOT EXISTS schema_version
    (
        version INT NOT NULL,
        updated_at TIMESTAMP CONSTRAINT schemaversion_updatedat_nn NOT NULL
    )"""


let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createSchemaSql) |> ignore

    dbConnection.Execute("INSERT INTO schema_version VALUES(0, now())")
    |> ignore

    ()
