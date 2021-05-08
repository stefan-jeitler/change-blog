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

let create (dbConnection: IDbConnection) = 
    dbConnection.ExecuteAsync(createLineSql)
    |> Async.AwaitTask
    |> Async.Ignore