module ProjectTable

open System.Data
open Dapper

let private createProjectSql = """
        CREATE TABLE IF NOT EXISTS project 
        (
        	id UUID CONSTRAINT project_id_pkey PRIMARY KEY,
        	account_id UUID CONSTRAINT project_accountid_nn NOT NULL,
        	versioning_scheme_id UUID CONSTRAINT project_versioningschemeid_nn NOT NULL,
        	"name" TEXT CONSTRAINT project_name_nn NOT NULL,
        	created_by_user UUID CONSTRAINT project_createdbyuser_nn NOT NULL,
        	closed_at TIMESTAMP,
        	created_at TIMESTAMP CONSTRAINT project_createdat_nn NOT NULL,
        	CONSTRAINT project_accountid_fkey FOREIGN KEY (account_id) REFERENCES account(id),
        	CONSTRAINT project_versioningschemeid_fkey FOREIGN KEY (versioning_scheme_id) REFERENCES versioning_scheme(id),
        	CONSTRAINT project_createdbyuser_fkey FOREIGN KEY (created_by_user) REFERENCES "user"(id)
        )
    """

let create (dbConnection: IDbConnection) = 
    dbConnection.ExecuteAsync(createProjectSql)
    |> Async.AwaitTask
    |> Async.Ignore