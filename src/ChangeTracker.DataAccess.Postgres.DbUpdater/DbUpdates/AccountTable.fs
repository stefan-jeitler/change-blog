namespace DbUpdater

module AccountTable =
    open System.Data
    open Dapper

    let create (dbConnection: IDbConnection) = async {
            let! tableExists = Db.tableExists dbConnection "account"

            match tableExists with
            | true -> ()
            | false ->
                let createAccountSql = """
                    CREATE TABLE account (
                    id UUID CONSTRAINT account_id_pkey PRIMARY KEY,
                    "name" TEXT CONSTRAINT account_name_nn NOT NULL,
                    default_versioning_scheme_id UUID,
                    deleted_at timestamp,
                    created_at timestamp CONSTRAINT account_createdat_nn NOT NULL,
                    CONSTRAINT account_name_unique UNIQUE ("name"))"""

                do! dbConnection.ExecuteAsync(createAccountSql) 
                    |> Async.AwaitTask 
                    |> Async.Ignore
                ()
        }

    let addVersioningSchemeForeignKey (dbConnection: IDbConnection) = async {
        let constraintName = "account_versioningschemeid_fkey"
        let! constraintExists = Db.constraintExists dbConnection "account" constraintName

        let createConstraintSql = """
            ALTER TABLE account ADD CONSTRAINT account_versioningschemeid_fkey 
            	FOREIGN KEY (default_versioning_scheme_id)
            		REFERENCES versioning_scheme(id)
            """

        match constraintExists with
        | true -> ()
        | false -> 
            do! dbConnection.ExecuteAsync(createConstraintSql)
                |> Async.AwaitTask
                |> Async.Ignore
    }
        
