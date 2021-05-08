module AccountTable

open System.Data
open Dapper

let private createAccountSql = """
    CREATE TABLE IF NOT EXISTS account 
    (
        id UUID CONSTRAINT account_id_pkey PRIMARY KEY,
        "name" TEXT CONSTRAINT account_name_nn NOT NULL,
        default_versioning_scheme_id UUID,
        deleted_at timestamp,
        created_at timestamp CONSTRAINT account_createdat_nn NOT NULL,
        CONSTRAINT account_name_unique UNIQUE ("name")
    )"""

let private createConstraintSql = """
    ALTER TABLE account ADD CONSTRAINT account_versioningschemeid_fkey
    	FOREIGN KEY (default_versioning_scheme_id)
    		REFERENCES versioning_scheme(id)
    """

let create (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createAccountSql)
    |> Async.AwaitTask
    |> Async.Ignore

let addVersioningSchemeForeignKey (dbConnection: IDbConnection) =
    async {
        let constraintName = "account_versioningschemeid_fkey"
        let! constraintExists = Db.constraintExists dbConnection constraintName

        match constraintExists with
        | true -> ()
        | false ->
            do!
                dbConnection.ExecuteAsync(createConstraintSql)
                |> Async.AwaitTask
                |> Async.Ignore
    }
