module RolePermissionTable

open System.Data
open Dapper

let private createRolePermissionSql = """
    CREATE TABLE IF NOT EXISTS role_permission
    (
    	id UUID,
    	"name" TEXT,
    	created_at TIMESTAMP CONSTRAINT rolepermission_createdat_nn NOT NULL,
    	CONSTRAINT rolepermission_id_name_pkey PRIMARY KEY (id, "name")
    )
    """

let create (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createRolePermissionSql)
    |> Async.AwaitTask
    |> Async.Ignore
