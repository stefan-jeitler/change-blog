module AccountUserTable

open System.Data
open Dapper

let private createAccountUserSql = """
		CREATE TABLE IF NOT EXISTS account_user
		(
			account_id UUID CONSTRAINT accountuser_accountid_nn NOT NULL,
			user_id UUID CONSTRAINT accountuser_userid_nn NOT NULL,
			role_id UUID CONSTRAINT accountuser_roleid_nn NOT NULL,
			created_at TIMESTAMP CONSTRAINT accountuser_createdat_nn NOT NULL,
			CONSTRAINT accountuser_accountid_userid_roleid_pkey PRIMARY KEY (account_id, user_id, role_id),
			CONSTRAINT accountuser_accountid_fkey FOREIGN KEY (account_id) REFERENCES account(id),
			CONSTRAINT accountuser_userid_fkey FOREIGN KEY (user_id) REFERENCES "user"(id),
			CONSTRAINT accountuser_roleid_fkey FOREIGN KEY (role_id) REFERENCES "role"(id)
		)
    """

let create (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createAccountUserSql)
    |> Async.AwaitTask
    |> Async.Ignore
