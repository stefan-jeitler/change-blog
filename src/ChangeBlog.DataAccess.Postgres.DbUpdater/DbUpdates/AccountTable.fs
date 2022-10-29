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

let private addChangeTrackerAccountSql =
    """INSERT INTO account
    (id, name, default_versioning_scheme_id, deleted_at, created_at)
    VALUES ('a00788cb-03f8-4a8c-84b6-756622550e8c', 'ChangeTracker', null, null, '2021-05-23 20:40:38.879023')
    on conflict (id) do nothing"""

let private fixUniqueIndexOnAccountNameSql =
    [ "CREATE UNIQUE INDEX IF NOT EXISTS account_name_unique ON account (LOWER(name)) where deleted_at is null"
      "drop index if exists account_name_deletedat_unique" ]

let private addCreatedByUserColumnSql =
    [ "ALTER TABLE account ADD COLUMN IF NOT EXISTS created_by_user UUID"
      """
        insert into "user" (id, email, first_name, last_name, timezone, deleted_at, created_at, culture)
        values ('f371efb3-a23f-4d8e-9318-5c77b435a250', 'dummy@changeblog.com', 'dummy', 'user', 'UTC', null, now(), 'en-US')
        on conflict do nothing
        """
      "UPDATE account SET created_by_user = 'f371efb3-a23f-4d8e-9318-5c77b435a250' WHERE created_by_user IS NULL"
      "ALTER TABLE account ALTER COLUMN created_by_user SET NOT NULL"
      """ALTER TABLE account ADD CONSTRAINT account_createdbyuser_fkey FOREIGN KEY (created_by_user) REFERENCES "user"(id)"""
      "CREATE INDEX IF NOT EXISTS account_createdbyuser_idx ON account (created_by_user)" ]

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createAccountSql) |> ignore

let addVersioningSchemeForeignKey (dbConnection: IDbConnection) =
    let constraintName =
        "account_versioningschemeid_fkey"

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

let addChangeTrackerAccount (dbConnection: IDbConnection) =
    dbConnection.Execute(addChangeTrackerAccountSql)
    |> ignore

let fixUniqueIndexOnAccountName (dbConnection: IDbConnection) =
    fixUniqueIndexOnAccountNameSql
    |> List.map dbConnection.Execute
    |> ignore

let addCreatedByUserColumn (dbConnection: IDbConnection) =
    addCreatedByUserColumnSql
    |> List.map dbConnection.Execute
    |> ignore
