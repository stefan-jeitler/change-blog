module Db

open System.Data
open Dapper
open Semver

let inline (=>) a b = a, box b

let tableExists (dbConnection: IDbConnection) (tableName: string) =
    let tableExistsSql =
        """
        SELECT EXISTS (
            SELECT FROM information_schema.tables
            WHERE table_name = @tableName
        )"""

    let parameters =
        dict [ "tableName" => tableName.ToLower() ]

    dbConnection.ExecuteScalar<bool>(tableExistsSql, parameters)

let constraintExists (dbConnection: IDbConnection) (constraintName: string) =
    let constraintExistsSql =
        """
        SELECT EXISTS(SELECT NULL
        FROM information_schema.constraint_column_usage
        WHERE CONSTRAINT_NAME = @constraintName)
    """

    let parameters =
        dict [ "constraintName" => constraintName.ToLower() ]

    dbConnection.ExecuteScalar<bool>(constraintExistsSql, parameters)

let getLatestSchemaVersion (dbConnection: IDbConnection) =
    let tableExists =
        tableExists dbConnection "schema_version"

    match tableExists with
    | false -> SemVersion.Parse("0.0.0", SemVersionStyles.Strict)
    | true ->
        let versionSql =
            "SELECT version FROM schema_version"

        let latestVersion =
            dbConnection.ExecuteScalar<string>(versionSql)

        if latestVersion = null then
            SemVersion.Parse("0.0.0", SemVersionStyles.Strict)
        else
            SemVersion.Parse(latestVersion, SemVersionStyles.Strict)

let updateSchemaVersion (dbConnection: IDbConnection) (version: SemVersion) =
    let updateSchemaVersionSql =
        "UPDATE schema_version SET version = @version, updated_at = now()"

    let insertSchemaVersionSql =
        "INSERT INTO schema_version (version, updated_at) VALUES(@version, now())"

    let param =
        dict [ "version" => version.ToString() ]

    let c =
        dbConnection.Execute(updateSchemaVersionSql, param)

    match c with
    | 0 ->
        dbConnection.Execute(insertSchemaVersionSql, param)
        |> ignore
    | _ -> ()

let getDbName (dbConnection: IDbConnection) =
    dbConnection.ExecuteScalar<string>("SELECT current_database()")
