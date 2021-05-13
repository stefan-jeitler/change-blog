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

let private dropProjectIdValueUniqueConstraintSql = """ALTER TABLE "version" DROP CONSTRAINT IF EXISTS version_projectid_value_unique"""

let private createProjectIdValueDeletedAtUniqueConstraintSql = """
        CREATE UNIQUE INDEX IF NOT EXISTS version_projectid_value_deletedatnull_unique
        ON "version" (project_id, value, (deleted_at IS NULL)) WHERE deleted_at is null
    """

let create (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createVersionSql)
    |> Async.AwaitTask
    |> Async.Ignore

let modifyUniqueConstraint (dbConnection: IDbConnection) =
    async {
        do! dbConnection.ExecuteAsync(dropProjectIdValueUniqueConstraintSql)
            |> Async.AwaitTask
            |> Async.Ignore

        do! dbConnection.ExecuteAsync(createProjectIdValueDeletedAtUniqueConstraintSql)
            |> Async.AwaitTask
            |> Async.Ignore
    }

