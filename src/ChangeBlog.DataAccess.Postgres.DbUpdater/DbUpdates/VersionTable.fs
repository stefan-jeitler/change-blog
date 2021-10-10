module VersionTable

open System.Data
open Dapper
open System
open Db

let private createVersionSql =
    """
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

let private addUniqueIndexProductIdValueDeletedAtSql =
    """
        CREATE UNIQUE INDEX IF NOT EXISTS version_productid_value_deletedatnull_unique
        ON "version" (product_id, lower(value), (deleted_at IS NULL)) WHERE deleted_at is null
    """

let private addSearchVectorsColumnSql =
    """ALTER TABLE version ADD COLUMN IF NOT EXISTS "search_vectors" tsvector"""

let private createSearchVectorsIndexSql =
    """CREATE INDEX IF NOT EXISTS version_searchvector_idx ON version USING gin (search_vectors)"""

let private dropTsVectorAggregateSql =
    "DROP AGGREGATE IF EXISTS tsvector_agg(tsvector)"

let private addTsVectorAggregateSql =
    """
        CREATE AGGREGATE tsvector_agg(tsvector) (
            STYPE = pg_catalog.tsvector,
            SFUNC = pg_catalog.tsvector_concat,
            INITCOND = ''
            )
    """

let private createUpdateSearchVectorsProcedureSql =
    """
        CREATE OR REPLACE PROCEDURE update_version_searchvectors_proc(versionId uuid)
        LANGUAGE SQL
        AS
        $$
        WITH chl_agg AS (
        select (select tsvector_agg(to_tsvector(text)
                                        || to_tsvector(regexp_replace((SELECT coalesce(string_agg(value::text, ' '), '')
                                                                        FROM jsonb_array_elements_text(chl.labels)),
                                                                        '([a-z])([A-Z])',
                                                                        '\1 \2',
                                                                        'g'))
            || to_tsvector((SELECT coalesce(string_agg(value::text, ' '), '')
                            FROM jsonb_array_elements_text(chl.labels || chl.issues))))
                ) as value
        from changelog_line chl
        where chl.version_id = versionId
        )
        update version v
        set search_vectors = (to_tsvector(v.value) || to_tsvector(v.name) || chl_agg.value)
        from chl_agg
        where v.id = versionId;
        $$
	"""

let private createUpdateAllSearchVectorsProcedureSql =
    """
        CREATE OR REPLACE PROCEDURE update_all_version_searchvectors_proc()
        LANGUAGE plpgsql
        AS
        $$
        DECLARE
        t_row version%rowtype;
        BEGIN
        FOR t_row in SELECT * FROM version
            LOOP
                Call update_version_searchvectors_proc(t_row.id);
            END LOOP;
        END;
        $$
    """

let private makeUpdateSearchVectorsProcedureLanguageAwareSql =
    """
        CREATE OR REPLACE PROCEDURE update_version_searchvectors_proc(versionId uuid)
        LANGUAGE plpgsql
        AS
        $$
        DECLARE
        lang regconfig;
        BEGIN
        select LOWER(l.name)
        into lang
        from language l
        join product p on l.code = p.language_code
        where p.id = (select v.product_id from version v where v.id = versionId)
        fetch first 1 row only ;

        RAISE NOTICE 'VersionId: %, Language: %', versionId, lang;

        WITH chl_agg AS (
            select (select tsvector_agg(to_tsvector(lang, text)
                                            ||
                                        to_tsvector(lang, regexp_replace((SELECT coalesce(string_agg(value::text, ' '), '')
                                                                               FROM jsonb_array_elements_text(chl.labels)),
                                                                              '([a-z])([A-Z])',
                                                                              '\1 \2',
                                                                              'g'))
                || to_tsvector(lang, (SELECT coalesce(string_agg(value::text, ' '), '')
                                           FROM jsonb_array_elements_text(chl.labels || chl.issues))))
                   ) as value
            from changelog_line chl
            where chl.version_id = versionId
        )
        update version v
        set search_vectors = (to_tsvector(lang, v.value) || to_tsvector(lang, v.name) || chl_agg.value)
        from chl_agg
        where v.id = versionId;
        END
        $$
    """

let private dropUpdateAllVersionSearchVectorsProcedureSql =
    "drop procedure if exists update_all_version_searchvectors_proc()"

let private fixUniqueIndexOnProductIdAndValueSql = 
    [
        """
            CREATE UNIQUE INDEX IF NOT EXISTS version_productid_value_unique
            ON "version" (product_id, lower(value)) WHERE deleted_at is null
        """
        "drop index if exists version_productid_value_deletedatnull_unique"
    ]

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
      dropTsVectorAggregateSql
      createSearchVectorsIndexSql
      addTsVectorAggregateSql
      createUpdateSearchVectorsProcedureSql
      createUpdateAllSearchVectorsProcedureSql ]
    |> executeSql

    dbConnection.Execute("CALL update_all_version_searchvectors_proc()")
    |> ignore

    ()

let makeUpdateSearchVectorsProcedureLanguageAware (dbConnection: IDbConnection) =
    dbConnection.Execute(makeUpdateSearchVectorsProcedureLanguageAwareSql)
    |> ignore

let dropUpdateAllVersionSearchVectorsProcedure (dbConnection: IDbConnection) =
    dbConnection.Execute(dropUpdateAllVersionSearchVectorsProcedureSql)
    |> ignore

let fixUniqueIndexOnProductIdAndValue (dbConnection: IDbConnection) =
    fixUniqueIndexOnProductIdAndValueSql
    |> List.map (fun x -> dbConnection.Execute(x))
    |> ignore