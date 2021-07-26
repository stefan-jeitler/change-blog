module UserTable

open System.Data
open Dapper

let private createUserSql = """
        CREATE TABLE IF NOT EXISTS "user"
        (
            id UUID CONSTRAINT user_id_pkey PRIMARY KEY,
            email TEXT CONSTRAINT user_email_nn NOT NULL,
            first_name TEXT CONSTRAINT user_firstname_nn NOT NULL,
            last_name TEXT CONSTRAINT user_lastname_nn NOT NULL,
            "timezone" TEXT CONSTRAINT user_timezone_nn NOT NULL,
            deleted_at TIMESTAMP,
            created_at TIMESTAMP CONSTRAINT user_deletedat_nn NOT NULL,
            CONSTRAINT user_email_unique UNIQUE (email)
        )
    """

let private createLowerEmailIndexSql =
    """CREATE INDEX IF NOT EXISTS user_email_idx ON "user" (lower(email))"""

let private addUserForDefaultVersioningSchemesSql = """
        insert into "user" (id, email, first_name, last_name, timezone, deleted_at, created_at)
        values ('a17f556c-14c3-4bc2-bfa7-3c0811fd75b8',
                'versioning.scheme@change-tracker.com',
                'versioning',
                'scheme',
                'Europe/Berlin',
                null,
                now())
        on conflict (id) do nothing 
    """

let private fixEmailUniqueConstraintSql = [
    "ALTER TABLE \"user\" DROP CONSTRAINT IF EXISTS user_email_unique"
    "DROP INDEX IF EXISTS user_email_idx"
    "CREATE UNIQUE INDEX IF NOT EXISTS user_email_idx ON \"user\" (lower(email))"
]

let private addUserForAppChangesSql = """
        insert into "user"
        values ('c1db054a-a07c-4712-9c69-86e0806a89a0',
                'changes@change-tracker.com',
                'change',
                'tracker',
                'UTC',
                null,
                now())
        on conflict (id) do nothing
    """

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createUserSql) |> ignore

    dbConnection.Execute(createLowerEmailIndexSql)
    |> ignore

    ()

let addUserForDefaultVersioningSchemes (dbConnection: IDbConnection) = 
    dbConnection.Execute(addUserForDefaultVersioningSchemesSql)
    |> ignore
    
let fixEmailUniqeConstraint (dbConnection: IDbConnection) = 
    fixEmailUniqueConstraintSql
    |> List.map (fun x -> dbConnection.Execute(x))
    |> ignore

let addUserForAppChanges (dbConnection: IDbConnection) = 
    dbConnection.Execute(addUserForAppChangesSql)
    |> ignore
