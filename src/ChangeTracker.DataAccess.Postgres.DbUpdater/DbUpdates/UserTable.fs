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

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createUserSql) |> ignore

    dbConnection.Execute(createLowerEmailIndexSql)
    |> ignore
