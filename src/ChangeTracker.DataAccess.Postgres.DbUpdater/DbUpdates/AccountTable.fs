namespace DbUpdater

module AccountTable =
    open System.Data
    open Dapper

    let create (dbConnection: IDbConnection) =
        async {
            let! tableExists = Db.tableExists dbConnection "schema_version"

            match tableExists with
            | true -> ()
            | false ->
                let createAccountSql = """
                  CREATE TABLE account
                  (
                    version INT NOT NULL,
                    updated_at TIMESTAMP NOT NULL
                  )"""

                ()
        }
