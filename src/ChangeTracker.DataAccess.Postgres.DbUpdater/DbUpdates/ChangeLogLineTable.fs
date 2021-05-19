﻿module ChangeLogLineTable

open System.Data
open Dapper

let private createLineSql = """
        CREATE TABLE IF NOT EXISTS changelog_line
        (
	        id UUID CONSTRAINT changelogline_id_pkey PRIMARY KEY,
	        version_id UUID,
	        product_id UUID CONSTRAINT changelogline_productid_nn NOT NULL,
	        "text" TEXT CONSTRAINT changelogline_text_nn NOT NULL,
	        labels JSONB CONSTRAINT changelogline_labels_nn NOT NULL,
	        issues JSONB CONSTRAINT changelogline_issues_nn NOT NULL,
	        "position" INTEGER CONSTRAINT changelogline_position_nn NOT NULL,
	        deleted_at TIMESTAMP,
	        created_at TIMESTAMP CONSTRAINT changelogline_createdat_nn NOT NULL,
	        CONSTRAINT changelogline_versionid_fkey FOREIGN KEY (version_id) REFERENCES "version"(id),
	        CONSTRAINT changelogline_productid_fkey FOREIGN KEY (product_id) REFERENCES product(id)
        )
    """

let private addPartialUniqueIndexOnProductIdVersionIdTextDeletedAtSql = """
        CREATE UNIQUE INDEX IF NOT EXISTS changelogline_productid_versionid_text_deletedat_unique
        ON changelog_line (product_id, version_id, LOWER("text"), (deleted_at is null)) where deleted_at is null
    """

let create (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createLineSql)
    |> Async.AwaitTask
    |> Async.Ignore

let addPartialUniqueIndexOnProductIdVersionIdTextDeletedAt (dbConnection: IDbConnection) = 
    dbConnection.ExecuteAsync(addPartialUniqueIndexOnProductIdVersionIdTextDeletedAtSql)
    |> Async.AwaitTask
    |> Async.Ignore
