namespace DbUpdater

module VersioningSchemeTable = 
    open System.Data
    open Dapper

    let create (dbConnection: IDbConnection) = async {
        let! tableExists = Db.tableExists dbConnection "versioning_scheme"

        match tableExists with
        | true -> ()
        | false -> 
            let createTableSql = """
                CREATE TABLE versioning_scheme (
                  id UUID CONSTRAINT versioningscheme_id_pkey PRIMARY KEY,
                  "name" TEXT CONSTRAINT versioningscheme_name_nn NOT NULL,
                  regex_pattern TEXT CONSTRAINT versioningscheme_regexpattern_nn NOT NULL,
                  description TEXT CONSTRAINT versioningscheme_description_nn NOT NULL,
                  account_id UUID,
                  deleted_at timestamp,
                  created_at timestamp CONSTRAINT account_createdat_nn NOT NULL,
                  CONSTRAINT versioningscheme_accountid_fkey FOREIGN KEY(account_id) REFERENCES account(id)
                )"""

            do! dbConnection.ExecuteAsync(createTableSql)
                |> Async.AwaitTask
                |> Async.Ignore
    }
        