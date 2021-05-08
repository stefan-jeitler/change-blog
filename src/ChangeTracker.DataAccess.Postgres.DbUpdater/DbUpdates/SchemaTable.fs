namespace DbUpdater

module SchemaTable =
    open System.Data
    open Dapper

    let create (dbConnection: IDbConnection) =
        async {
            let! tableExists = Db.tableExists dbConnection "schema_version"

            match tableExists with
            | true -> ()
            | false ->

                let createSchemaTableSql = """
                    CREATE TABLE schema_version
                    (
                      version INT NOT NULL,
                      updated_at TIMESTAMP NOT NULL
                    )"""

                do!
                    dbConnection.ExecuteAsync(createSchemaTableSql)
                    |> Async.AwaitTask
                    |> Async.Ignore

                do!
                    dbConnection.ExecuteAsync("INSERT INTO schema_version VALUES(0, now())")
                    |> Async.AwaitTask
                    |> Async.Ignore
        }
