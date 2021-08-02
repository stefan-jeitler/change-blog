module AccountTable

open System.Data
open Dapper

let private createAccountSql =
    """
    CREATE TABLE IF NOT EXISTS account
    (
        id UUID CONSTRAINT account_id_pkey PRIMARY KEY,
        "name" TEXT CONSTRAINT account_name_nn NOT NULL,
        default_versioning_scheme_id UUID,
        deleted_at timestamp,
        created_at timestamp CONSTRAINT account_createdat_nn NOT NULL
    )"""

let private createConstraintSql =
    """
    ALTER TABLE account ADD CONSTRAINT account_versioningschemeid_fkey
    	FOREIGN KEY (default_versioning_scheme_id)
    		REFERENCES versioning_scheme(id)
    """

let private addUniqueIndexOnNameAndDeletedAtSql =
    "CREATE UNIQUE INDEX IF NOT EXISTS account_name_deletedat_unique ON account (LOWER(name), (deleted_at is null)) where deleted_at is null"

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createAccountSql) |> ignore

let addVersioningSchemeForeignKey (dbConnection: IDbConnection) =
    let constraintName = "account_versioningschemeid_fkey"

    let constraintExists =
        Db.constraintExists dbConnection constraintName

    match constraintExists with
    | true -> ()
    | false ->
        dbConnection.Execute(createConstraintSql)
        |> ignore

let addPartialUniqueIndexOnNameAndDeletedAt (dbConnection: IDbConnection) =
    dbConnection.Execute(addUniqueIndexOnNameAndDeletedAtSql)
    |> ignore
