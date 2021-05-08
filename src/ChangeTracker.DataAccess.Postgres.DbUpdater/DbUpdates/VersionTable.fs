namespace DbUpdater

module VersionTable =
    open System.Data
    open Dapper

    let create (dbConnection: IDbConnection) =
        async {

            let! tableExists = Db.tableExists dbConnection "version"

            match tableExists with
            | true -> ()
            | false ->
                let createVersionSql = """
                    CREATE TABLE version
                    (
                      id UUID NOT NULL,
                      created_at TIMESTAMP NOT NULL
                    )"""

                do!
                    dbConnection.ExecuteAsync(createVersionSql)
                    |> Async.AwaitTask
                    |> Async.Ignore
        }
