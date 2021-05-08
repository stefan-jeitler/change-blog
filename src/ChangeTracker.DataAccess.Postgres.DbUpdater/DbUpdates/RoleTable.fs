module RoleTable

open System.Data
open Dapper

let private createRoleSql = """
        CREATE TABLE IF NOT EXISTS "role"
        (
        	id UUID CONSTRAINT role_id_pkey PRIMARY KEY,
        	"name" TEXT CONSTRAINT role_name_nn NOT NULL,
        	description TEXT CONSTRAINT role_description_nn NOT NULL,
        	created_at TIMESTAMP CONSTRAINT role_createdat_nn NOT NULL,
        	CONSTRAINT role_name_unique UNIQUE ("name")
        )
    """

let create (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createRoleSql)
    |> Async.AwaitTask
    |> Async.Ignore
