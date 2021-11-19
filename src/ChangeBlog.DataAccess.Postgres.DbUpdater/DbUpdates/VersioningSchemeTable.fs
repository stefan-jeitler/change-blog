module VersioningSchemeTable

open System.Data
open Dapper

let private createVersioningSchemeSql =
    """
        CREATE TABLE IF NOT EXISTS versioning_scheme (
            id UUID CONSTRAINT versioningscheme_id_pkey PRIMARY KEY,
            "name" TEXT CONSTRAINT versioningscheme_name_nn NOT NULL,
            regex_pattern TEXT CONSTRAINT versioningscheme_regexpattern_nn NOT NULL,
            description TEXT CONSTRAINT versioningscheme_description_nn NOT NULL,
            account_id UUID,
            created_by_user UUID CONSTRAINT versioningscheme_createdbyuser_nn NOT NULL,
            deleted_at timestamp,
            created_at timestamp CONSTRAINT versioningscheme_createdat_nn NOT NULL,
            CONSTRAINT versioningscheme_accountid_fkey FOREIGN KEY(account_id) REFERENCES account(id),
            CONSTRAINT versioningscheme_createdbyuser_fkey FOREIGN KEY (created_by_user) REFERENCES "user"(id)
        )
    """

let private insertSemVer2SchemeSql =
    """
        INSERT INTO versioning_scheme VALUES ('4091b948-9bc5-43ee-9f98-df3d27853565',
        'SemVer 2.0.0',
        '^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$',
        'Semantic Versioning 2.0.0 - see https://semver.org/ for more info',
        NULL,
        'a17f556c-14c3-4bc2-bfa7-3c0811fd75b8',
        NULL,
        '2021-05-08T16:18:00Z'::timestamptz)
        ON CONFLICT (id)
        DO NOTHING
    """

let private addUniqueIndexOnNameAccountIdDeletedAtSql =
    """
         CREATE UNIQUE INDEX IF NOT EXISTS versioningscheme_name_accountId_deleted_unique
         on versioning_scheme (Lower("name"), account_id, (deleted_at is null)) where deleted_at is null
    """

let private fixUniqueIndexOnNameAndAccountIdSql = 
    [
        """
            CREATE UNIQUE INDEX IF NOT EXISTS versioningscheme_name_accountId_unique
            on versioning_scheme (Lower("name"), account_id) where deleted_at is null
        """
        "drop index if exists versioningscheme_name_accountId_deleted_unique"
    ]

let create (dbConnection: IDbConnection) =
    dbConnection.Execute(createVersioningSchemeSql)
    |> ignore

let addSemVer2DefaultScheme (dbConnection: IDbConnection) =
    dbConnection.Execute(insertSemVer2SchemeSql)
    |> ignore

let addUniqueIndexOnNameAccountIdDeletedAt (dbConnection: IDbConnection) =
    dbConnection.Execute(addUniqueIndexOnNameAccountIdDeletedAtSql)
    |> ignore

let fixUniqueIndexOnNameAndAccountId (dbConnection: IDbConnection) = 
    fixUniqueIndexOnNameAndAccountIdSql
    |> List.map dbConnection.Execute
    |> ignore