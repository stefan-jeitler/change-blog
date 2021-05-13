module ChangeLogLineTable

open System.Data
open Dapper

let private createLineSql = """
        CREATE TABLE IF NOT EXISTS changelog_line
        (
        	id UUID CONSTRAINT changelogline_id_pkey PRIMARY KEY,
        	version_id UUID,
        	project_id UUID CONSTRAINT changelogline_projectid_nn NOT NULL,
        	"text" TEXT CONSTRAINT changelogline_text_nn NOT NULL,
        	properties JSONB CONSTRAINT changelogline_properties_nn NOT NULL,
        	"position" INTEGER CONSTRAINT changelogline_position_nn NOT NULL,
        	deleted_at TIMESTAMP,
        	created_at TIMESTAMP CONSTRAINT changelogline_createdat_nn NOT NULL,
        	CONSTRAINT changelogline_versionid_fkey FOREIGN KEY (version_id) REFERENCES "version"(id),
        	CONSTRAINT changelogline_projectid_fkey FOREIGN KEY (project_id) REFERENCES project(id),
        	CONSTRAINT changelogline_projectid_versionid_text UNIQUE (project_id, version_id, "text")
        )
    """

let private dropUniqueConstraintSql = "ALTER TABLE changelog_line DROP CONSTRAINT IF EXISTS changelogline_projectid_versionid_text"

let private addUniqueIndexOnProjectIdVersionIdTextDeletedAtSql = """
        CREATE UNIQUE INDEX IF NOT EXISTS changelogline_projectid_versionid_text_deletedat_unique
        ON changelog_line (project_id, version_id, LOWER("text"), (deleted_at is null)) where deleted_at is null
    """

let create (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createLineSql)
    |> Async.AwaitTask
    |> Async.Ignore

let fixUniqueIndexConstraint (dbConnection: IDbConnection) = 
    async {
        do! dbConnection.ExecuteAsync(dropUniqueConstraintSql)
            |> Async.AwaitTask
            |> Async.Ignore

        do! dbConnection.ExecuteAsync(addUniqueIndexOnProjectIdVersionIdTextDeletedAtSql)
            |> Async.AwaitTask
            |> Async.Ignore
    }