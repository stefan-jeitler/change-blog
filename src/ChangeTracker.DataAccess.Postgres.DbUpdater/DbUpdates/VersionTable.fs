module VersionTable

open System.Data
open Dapper

let private createVersionSql = """
        CREATE TABLE IF NOT EXISTS "version"
        (
        	id UUID CONSTRAINT version_id_pkey PRIMARY KEY,
        	project_id UUID CONSTRAINT version_projectid_nn NOT NULL,
        	"value" TEXT CONSTRAINT version_value_nn NOT NULL,
        	released_at TIMESTAMP, 
        	deleted_at TIMESTAMP, 
        	created_at TIMESTAMP CONSTRAINT version_createad_nn NOT NULL,
        	CONSTRAINT version_projectid_fkey FOREIGN KEY (project_id) REFERENCES project(id),
        	CONSTRAINT version_projectid_value_unique UNIQUE (project_id, "value")
        )
    """

let create (dbConnection: IDbConnection) = 
    dbConnection.ExecuteAsync(createVersionSql)
    |> Async.AwaitTask
    |> Async.Ignore