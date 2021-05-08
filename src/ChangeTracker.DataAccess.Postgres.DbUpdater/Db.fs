namespace DbUpdater

module Db =
    open System.Data
    open Dapper

    let inline (=>) a b = a, box b

    let tableExists (dbConnection: IDbConnection) (tableName: string) = async {
            let tableExistsSql = """
              SELECT EXISTS (
                 SELECT FROM information_schema.tables
                 WHERE table_name = @tableName
              )"""

            let parameters = dict [ "tableName" => tableName.ToLower() ]

            return! dbConnection.ExecuteScalarAsync<bool>(tableExistsSql, parameters)
                    |> Async.AwaitTask
        }

    let constraintExists (dbConnection: IDbConnection) (tableName: string) (constraintName: string) = async {
        let constraintExistsSql = """
                select CASE WHEN COUNT(*) = 1 THEN 'true' ELSE 'false' END
                from information_schema.constraint_column_usage 
                where TABLE_NAME = @tableName  and CONSTRAINT_NAME = @constraintName
            """

        let parameters = dict [ "tableName" => tableName.ToLower(); "constraintName" => constraintName.ToLower()]

        return! dbConnection.ExecuteScalarAsync<bool>(constraintExistsSql, parameters)
                |> Async.AwaitTask
        }

    let getLatestSchemaVersion (dbConnection: IDbConnection) =
        async {
            let! tableExists = tableExists dbConnection "schema_version"

            match tableExists with
            | false -> return -1
            | true ->
                let versionSql = "SELECT version FROM schema_version"

                return! dbConnection.ExecuteScalarAsync<int>(versionSql)
                        |> Async.AwaitTask
        }

    let updateSchemaVersion (dbConnection: IDbConnection) (version: int) = async {
            let updateSchemaVersionSql =
                "UPDATE schema_version SET version = @version, updated_at = now()"

            let insertSchemaVersionSql =
                "INSERT INTO schema_version (version, updated_at) VALUES(@version, now())"

            let param = dict [ "version" => version ]

            let! c =
                dbConnection.ExecuteAsync(updateSchemaVersionSql, param)
                |> Async.AwaitTask

            match c with
            | 0 ->
                do!
                    dbConnection.ExecuteAsync(insertSchemaVersionSql, param)
                    |> Async.AwaitTask
                    |> Async.Ignore
            | _ -> ()
        }

    let getDbName (dbConnection: IDbConnection) =
        dbConnection.ExecuteScalarAsync<string>("SELECT current_database()")
        |> Async.AwaitTask
