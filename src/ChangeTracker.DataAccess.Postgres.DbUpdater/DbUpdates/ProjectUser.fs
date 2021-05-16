module ProjectUser

open System.Data
open Dapper

let private createProjectUserRoleSql = """
		CREATE TABLE IF NOT EXISTS project_user
		(
			project_id UUID,
			user_id UUID,
			role_id UUID,
			created_at TIMESTAMP CONSTRAINT projectuserrole_createdat_nn NOT NULL,
			CONSTRAINT projectuserrole_userid_projectid_roleid_pkey PRIMARY KEY (project_id, user_id, role_id),
			CONSTRAINT projectuserrole_userid_fkey FOREIGN KEY (user_id) REFERENCES "user"(id),
			CONSTRAINT projectuserrole_projectid_fkey FOREIGN KEY (project_id) REFERENCES project(id),
			CONSTRAINT projectuserrole_roleid_fkey FOREIGN KEY (role_id) REFERENCES "role"(id)
		)
    """

let create (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createProjectUserRoleSql)
    |> Async.AwaitTask
    |> Async.Ignore
