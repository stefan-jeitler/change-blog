module AccountUserRoleTable

open System.Data
open Dapper

let private createAccountUserRoleSql = """
        CREATE TABLE IF NOT EXISTS account_user_role
        (
        	account_user_id UUID,
        	role_id UUID,
        	created_at TIMESTAMP CONSTRAINT accountuserrole_createdat_nn NOT NULL,
        	CONSTRAINT accountuserrole_accountuserid_roleid_pkey PRIMARY KEY (account_user_id, role_id),
        	CONSTRAINT accountuserrole_accountuserid_fkey FOREIGN KEY (account_user_id) REFERENCES account_user(id),
        	CONSTRAINT accountuserrole_roleid_fkey FOREIGN KEY (role_id) REFERENCES "role"(id)
        )
    """

let create (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createAccountUserRoleSql)
    |> Async.AwaitTask
    |> Async.Ignore
