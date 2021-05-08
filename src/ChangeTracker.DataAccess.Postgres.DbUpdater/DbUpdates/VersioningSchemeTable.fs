module VersioningSchemeTable

open System.Data
open Dapper

let private createVersioningSchemeSql = """
    CREATE TABLE IF NOT EXISTS versioning_scheme (
        id UUID CONSTRAINT versioningscheme_id_pkey PRIMARY KEY,
        "name" TEXT CONSTRAINT versioningscheme_name_nn NOT NULL,
        regex_pattern TEXT CONSTRAINT versioningscheme_regexpattern_nn NOT NULL,
        description TEXT CONSTRAINT versioningscheme_description_nn NOT NULL,
        account_id UUID,
        deleted_at timestamp,
        created_at timestamp CONSTRAINT account_createdat_nn NOT NULL,
        CONSTRAINT versioningscheme_accountid_fkey FOREIGN KEY(account_id) REFERENCES account(id)
    )"""

let private insertSemVer2Scheme = """
        INSERT INTO versioning_scheme VALUES ('4091b948-9bc5-43ee-9f98-df3d27853565',
        'SemVer 2.0.0',
        '^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$',
        'Semantic Versioning 2.0.0 - see https://semver.org/ for more info',
        NULL,
        NULL,
        '2021-05-08T16:18:00Z'::timestamptz)
        ON CONFLICT (id)
        DO NOTHING
    """

let create (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(createVersioningSchemeSql)
    |> Async.AwaitTask
    |> Async.Ignore

let addSemVer2DefaultScheme (dbConnection: IDbConnection) =
    dbConnection.ExecuteAsync(insertSemVer2Scheme)
    |> Async.AwaitTask
    |> Async.Ignore
