module VersionTable

open System.Data
open Dapper

let private createVersionSql = """
        CREATE TABLE IF NOT EXISTS "version"
        (
        	id UUID CONSTRAINT version_id_pkey PRIMARY KEY,
        	product_id UUID CONSTRAINT version_productid_nn NOT NULL,
        	"value" TEXT CONSTRAINT version_value_nn NOT NULL,
        	released_at TIMESTAMP,
        	deleted_at TIMESTAMP,
        	created_at TIMESTAMP CONSTRAINT version_createad_nn NOT NULL,
        	CONSTRAINT version_productid_fkey FOREIGN KEY (product_id) REFERENCES product(id)
        )
    """

let private addUniqueIndexProductIdValueDeletedAtSql = """
        CREATE UNIQUE INDEX IF NOT EXISTS version_productid_value_deletedatnull_unique
        ON "version" (product_id, lower(value), (deleted_at IS NULL)) WHERE deleted_at is null
    """

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createVersionSql) |> ignore

let addUniqueIndexProductIdValueDeletedAt (dbConnection: IDbConnection) =
    dbConnection.Execute(addUniqueIndexProductIdValueDeletedAtSql)
    |> ignore
