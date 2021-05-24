module ChangeLogLineTable

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
        created_by_user UUID CONSTRAINT changelogline_createdbyuser_nn NOT NULL,
        deleted_at TIMESTAMP,
        created_at TIMESTAMP CONSTRAINT changelogline_createdat_nn NOT NULL,
        CONSTRAINT changelogline_versionid_fkey FOREIGN KEY (version_id) REFERENCES "version"(id),
        CONSTRAINT changelogline_productid_fkey FOREIGN KEY (product_id) REFERENCES product(id),
        CONSTRAINT changelogline_createdbyuser_fkey FOREIGN KEY (created_by_user) REFERENCES "user"(id)
     )
    """

let private addPartialUniqueIndexOnProductIdVersionIdTextDeletedAtSql = """
        CREATE UNIQUE INDEX IF NOT EXISTS changelogline_productid_versionid_text_deletedat_unique
        ON changelog_line (product_id, version_id, LOWER("text"), (deleted_at is null)) where deleted_at is null
    """

let private addSearchVectorsColumnSql =
    """ALTER TABLE changelog_line ADD IF NOT EXISTS "search_vectors" tsvector"""

let private createSearchVectorsIndexSql =
    """CREATE INDEX IF NOT EXISTS changelogline_searchvector_idx ON changelog_line USING gin (search_vectors)"""

let private createUpdateTextSearchVectorsFunctionSql = """
        CREATE OR REPLACE FUNCTION update_changelogline_textsearch() RETURNS trigger AS
        $$
        DECLARE
        BEGIN
            NEW.search_vectors = (to_tsvector((select coalesce(v.value, '') from version v where v.id = NEW.version_id))
                || (to_tsvector((select coalesce(v.name, '') from version v where v.id = NEW.version_id)))
                || to_tsvector(NEW.text)
                || to_tsvector(regexp_replace((SELECT coalesce(string_agg(value::text, ' '), '')
                                               FROM jsonb_array_elements_text(NEW.labels || NEW.issues)), '([a-z])([A-Z])',
                                              '\1 \2',
                                              'g')));

            RETURN NEW;
        END;
        $$ LANGUAGE plpgsql
	"""

let private dropUpdateTextSearchTriggerSql =
    "drop trigger if exists update_changelogline_textsearch on changelog_line"

let private createUpdateTextSearchTriggerSql = """
		CREATE TRIGGER update_changelogline_textsearch
		    BEFORE INSERT OR UPDATE
		    ON changelog_line
		    FOR EACH ROW
		EXECUTE PROCEDURE update_changelogline_textsearch()
	"""

let private updateSearchVectorsForExistingLinesSql = """
        update changelog_line chl
        set search_vectors = (to_tsvector((select coalesce(v.value, '') from version v where v.id = chl.version_id))
            || (to_tsvector((select v.name from version v where v.id = chl.version_id)))
            || to_tsvector(text)
            || to_tsvector(regexp_replace((SELECT coalesce(string_agg(value::text, ' '), '')
                                           FROM jsonb_array_elements_text(chl.labels || chl.issues)), '([a-z])([A-Z])', '\1 \2',
                                          'g')))
    """

let private removeColumnsFromUpdateSearchVectorsFunctionSql = """
        CREATE OR REPLACE FUNCTION update_changelogline_textsearch() RETURNS trigger AS
        $$
        DECLARE
        BEGIN
            NEW.search_vectors = to_tsvector(NEW.text)
                || to_tsvector(regexp_replace((SELECT coalesce(string_agg(value::text, ' '), '')
                                               FROM jsonb_array_elements_text(NEW.labels || NEW.issues)), '([a-z])([A-Z])',
                                              '\1 \2',
                                              'g'));

            RETURN NEW;
        END;
        $$ LANGUAGE plpgsql
    """

let private updateSearchVectorsForExistingLinesAfterRemovingColumnsSql = """
    update changelog_line chl
    set search_vectors = to_tsvector(text)
        || to_tsvector(regexp_replace((SELECT coalesce(string_agg(value::text, ''), '')
                                       FROM jsonb_array_elements_text(chl.labels || chl.issues)), '([a-z])([A-Z])', '\1 \2',
                                      'g'))
    
"""

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createLineSql) |> ignore

let addPartialUniqueIndexOnProductIdVersionIdTextDeletedAt (dbConnection: IDbConnection) =
    dbConnection.Execute(addPartialUniqueIndexOnProductIdVersionIdTextDeletedAtSql)
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

let removeVersionColumnsFromTextSearch (dbConnection: IDbConnection) = 
    dbConnection.Execute(removeColumnsFromUpdateSearchVectorsFunctionSql) |> ignore 
    dbConnection.Execute(updateSearchVectorsForExistingLinesAfterRemovingColumnsSql) |> ignore 

    ()
