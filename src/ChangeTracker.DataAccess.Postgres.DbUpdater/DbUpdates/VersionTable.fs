module VersionTable

open System.Data
open Dapper

let private createVersionSql = """
		CREATE TABLE IF NOT EXISTS "version"
		(
			id UUID CONSTRAINT version_id_pkey PRIMARY KEY,
			product_id UUID CONSTRAINT version_productid_nn NOT NULL,
			"value" TEXT CONSTRAINT version_value_nn NOT NULL,
			name TEXT,
			released_at TIMESTAMP,
			created_by_user UUID CONSTRAINT version_createdbyuser_nn NOT NULL,
			deleted_at TIMESTAMP,
			created_at TIMESTAMP CONSTRAINT version_createad_nn NOT NULL,
			CONSTRAINT version_productid_fkey FOREIGN KEY (product_id) REFERENCES product(id),
			CONSTRAINT version_createdbyuser_fkey FOREIGN KEY (created_by_user) REFERENCES "user"(id)
		)
    """

let private addUniqueIndexProductIdValueDeletedAtSql = """
        CREATE UNIQUE INDEX IF NOT EXISTS version_productid_value_deletedatnull_unique
        ON "version" (product_id, lower(value), (deleted_at IS NULL)) WHERE deleted_at is null
    """

let private addSearchVectorsColumnSql = """ALTER TABLE version ADD COLUMN IF NOT EXISTS "search_vectors" tsvector"""

let private createSearchVectorsIndexSql = """CREATE INDEX IF NOT EXISTS version_searchvector_idx ON version USING gin (search_vectors)"""

let private createUpdateTextSearchVectorsFunctionSql = """
		CREATE OR REPLACE FUNCTION update_version_textsearch() RETURNS trigger AS
		$$
		DECLARE
		BEGIN
			NEW.search_vectors = (setweight(to_tsvector(NEW.value), 'A')
				|| (setweight(to_tsvector(NEW.name), 'B')));

			RETURN NEW;
		END;
		$$ LANGUAGE plpgsql	
	"""

let private dropUpdateTextSearchTriggerSql = """drop trigger if exists update_version_textsearch on version"""

let private createUpdateTextSearchTriggerSql = """
		CREATE TRIGGER update_version_textsearch
		BEFORE INSERT OR UPDATE
		ON version
		FOR EACH ROW
		EXECUTE PROCEDURE update_version_textsearch()
	"""

let private updateSearchVectorsForExistingLinesSql = """
		update version v
		set search_vectors = (setweight(to_tsvector(v.value), 'A')
				|| (setweight(to_tsvector(v.name), 'B')))
	"""

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createVersionSql) |> ignore

let addUniqueIndexProductIdValueDeletedAt (dbConnection: IDbConnection) =
    dbConnection.Execute(addUniqueIndexProductIdValueDeletedAtSql)
    |> ignore

let addTextSearch (dbConnection: IDbConnection) =
    let rec executeSql (statements: string list) =
        match statements with
        | [] -> ()
        | head :: tail ->
            dbConnection.Execute(head) |> ignore
            executeSql tail

    [ addSearchVectorsColumnSql
      createSearchVectorsIndexSql
      createUpdateTextSearchVectorsFunctionSql
      dropUpdateTextSearchTriggerSql
      createUpdateTextSearchTriggerSql
      updateSearchVectorsForExistingLinesSql ]
    |> executeSql

    ()