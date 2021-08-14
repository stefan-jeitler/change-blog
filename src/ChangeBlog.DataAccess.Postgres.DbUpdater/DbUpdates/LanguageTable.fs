module LanguageTable

open System.Data
open Dapper

let private createLanguageTableSql =
    """
        CREATE TABLE IF NOT EXISTS language
        (
            code char(2) CONSTRAINT language_code_pkey PRIMARY KEY,
            name text CONSTRAINT language_name_nn NOT NULL,
            created_at TIMESTAMP CONSTRAINT language_createdat_nn NOT NULL
        )
    """

let private addGermanSql =
    "INSERT INTO language VALUES('de', 'German', now()) ON CONFLICT (code) DO NOTHING"

let private addEnglishSql =
    "INSERT INTO language VALUES('en', 'English', now()) ON CONFLICT (code) DO NOTHING"

let addLanguageTableWithBasicLanguages (connection: IDbConnection) =
    connection.Execute(createLanguageTableSql)
    |> ignore

    connection.Execute(addGermanSql) |> ignore
    connection.Execute(addEnglishSql) |> ignore

    ()
