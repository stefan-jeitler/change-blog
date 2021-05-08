namespace DbUpdater

module ChangeLogLineTable =
    open System.Data
    open Dapper

    let create (dbConnection: IDbConnection) =
        async {
            let! tableExists = Db.tableExists dbConnection "changelog_line"

            match tableExists with
            | true -> ()
            | false ->
                let createChangeLogLineSql = """
                  CREATE TABLE changelog_line
                  (
                    ID UUID NOT NULL,
                    created_at TIMESTAMP NOT NULL
                  )"""

                do!
                    dbConnection.ExecuteAsync(createChangeLogLineSql)
                    |> Async.AwaitTask
                    |> Async.Ignore
        }
