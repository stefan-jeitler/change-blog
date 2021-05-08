module AccountUserTable

open System.Data
open Dapper

let private createAccountUserSql = """
        CREATE TABLE IF NOT EXISTS account_user
        (
        	id UUID CONSTRAINT accountuser_id_pkey PRIMARY KEY,
        	account_id UUID CONSTRAINT accountuser_accountid_nn NOT NULL,
        	user_id UUID CONSTRAINT accountuser_userid_nn NOT NULL,
        	created_at TIMESTAMP CONSTRAINT accountuser_createdat_nn NOT NULL,
        	CONSTRAINT accountuser_accountid_userid_unique UNIQUE (account_id, user_id),
        	CONSTRAINT accountuser_accountid_fkey FOREIGN KEY (account_id) REFERENCES account(id),
        	CONSTRAINT accountuser_userid_fkey FOREIGN KEY (user_id) REFERENCES "user"(id)
        )
    """

let create (dbConnection: IDbConnection) = 
    dbConnection.ExecuteAsync(createAccountUserSql)
    |> Async.AwaitTask
    |> Async.Ignore