module RolePermissionTable

open System.Data
open Dapper

let private createRolePermissionSql = """
		CREATE TABLE IF NOT EXISTS role_permission
		(
			role_id UUID,
			permission TEXT,
			created_at TIMESTAMP CONSTRAINT rolepermission_createdat_nn NOT NULL,
			CONSTRAINT rolepermission_roleid_permission_pkey PRIMARY KEY (role_id, permission),
			CONSTRAINT rolepermission_roleid_fkey FOREIGN KEY (role_id) REFERENCES "role"(id)
		)
    """

let private addPermissionAddProjectSql = """
		INSERT INTO role_permission
		SELECT id, 'AddProject', now() from role where name in ('ProductOwner', 'PlatformManager', 'Developer')
		ON CONFLICT (role_id, permission) DO NOTHING
	"""

let private addPermissionViewChangeLogLinesSql = """
		INSERT INTO role_permission
		SELECT id, 'ViewChangeLogLines', now() FROM role
		ON CONFLICT (role_id, permission) DO NOTHING
	"""

let create (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createRolePermissionSql)
    |> Async.AwaitTask
    |> Async.Ignore

let addPermissionAddProject (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(addPermissionAddProjectSql)
    |> Async.AwaitTask
    |> Async.Ignore

let addPermissionViewChangeLogLines (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(addPermissionViewChangeLogLinesSql)
    |> Async.AwaitTask
    |> Async.Ignore